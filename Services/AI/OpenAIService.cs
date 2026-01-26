using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using TravelTechApi.Common.Settings;
using TravelTechApi.DTOs.AI;

namespace TravelTechApi.Services.AI
{
    public class OpenAIService : IAIService
    {
        private readonly AISettings _aiSettings;
        private readonly ChatClient _chatClient;
        private readonly ILogger<OpenAIService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;

        public OpenAIService(
            IOptions<AISettings> aiSettings,
            ILogger<OpenAIService> logger)
        {
            _aiSettings = aiSettings.Value;
            _logger = logger;

            var apiKey = new ApiKeyCredential(_aiSettings.ApiKey);
            _chatClient = new ChatClient(_aiSettings.Model, apiKey);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public string GetModel() => _aiSettings.Model;

        #region Multi-step Generation (New Approach)

        /// <summary>
        /// Main entry point for multi-step generation
        /// </summary>
        public async Task<AIFullPlanResponse> GenerateFullPlanAsync(AIGenerationContext context)
        {
            _logger.LogInformation("Starting multi-step generation for {Location}, {Duration} days",
                context.LocationName, context.Duration);

            // Step 1: Generate outline
            var outline = await GeneratePlanOutlineAsync(context);

            // Step 2: Generate daily details in parallel (with semaphore to limit concurrency)
            var dailyDetails = new List<AIDailyDetailResponse>();
            var semaphore = new SemaphoreSlim(2); // Max 2 concurrent requests

            var tasks = outline.DailyOverview.Select(async day =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await GenerateDailyDetailWithRetryAsync(
                        context, day.DayNumber, day.Theme, day.Highlights);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            dailyDetails.AddRange(results.OrderBy(d => d.DayNumber));

            // Step 3: Aggregate results
            var fullPlan = new AIFullPlanResponse
            {
                Summary = outline.Summary,
                TotalEstimatedCostFrom = outline.TotalEstimatedCostFrom,
                TotalEstimatedCostTo = outline.TotalEstimatedCostTo,
                Accommodations = outline.Accommodations,
                Transportations = outline.Transportations,
                DailyItineraries = dailyDetails
            };

            _logger.LogInformation("Multi-step generation completed. Generated {Days} days",
                dailyDetails.Count);

            return fullPlan;
        }

        /// <summary>
        /// Step 1: Generate plan outline
        /// </summary>
        public async Task<AIPlanOutlineResponse> GeneratePlanOutlineAsync(AIGenerationContext context)
        {
            var systemPrompt = @"Bạn là chuyên gia du lịch Việt Nam. Nhiệm vụ: tạo OUTLINE tổng quan cho chuyến đi.

QUAN TRỌNG:
- Chỉ trả về JSON, không markdown
- Đề xuất địa điểm THỰC TẾ với giá cả chính xác
- DailyOverview chỉ cần theme và highlights ngắn gọn (sẽ chi tiết ở bước sau)

NGÂN SÁCH (VNĐ/ngày/người):
- Rẻ: <500,000
- Trung bình: 500,000-1,500,000
- Cao: >1,500,000";

            var userPrompt = BuildOutlinePrompt(context);

            var response = await CallAIWithRetryAsync(systemPrompt, userPrompt, "outline");
            return JsonSerializer.Deserialize<AIPlanOutlineResponse>(response, _jsonOptions)
                ?? throw new Exception("Failed to parse outline response");
        }

        /// <summary>
        /// Step 2: Generate daily detail for a specific day
        /// </summary>
        public async Task<AIDailyDetailResponse> GenerateDailyDetailAsync(
            AIGenerationContext context,
            int dayNumber,
            string dayTheme,
            List<string> highlights)
        {
            var systemPrompt = $@"Bạn là chuyên gia du lịch Việt Nam. Nhiệm vụ: tạo lịch trình CHI TIẾT cho NGÀY {dayNumber}.

QUAN TRỌNG:
- Chỉ trả về JSON, không markdown
- BẮT BUỘC 3 hoạt động với mô tả chi tiết (30-50 từ mỗi hoạt động)
- BẮT BUỘC 2-3 món ăn (sáng, trưa, tối hoặc ăn vặt)
- Thời gian thực tế, địa điểm THẬT
- Tips cụ thể từ người bản địa

IMAGE URL FORMAT (BẮT BUỘC):
- Địa điểm: https://tse1.mm.bing.net/th?q={{tên địa điểm}} + {{thành phố}}
- Món ăn: https://tse1.mm.bing.net/th?q={{tên món}} + {{tên quán}}

MAP URL FORMAT: https://www.google.com/maps/search/{{tên địa điểm}}+{{địa chỉ}}";

            var userPrompt = BuildDailyDetailPrompt(context, dayNumber, dayTheme, highlights);

            var response = await CallAIWithRetryAsync(systemPrompt, userPrompt, $"day{dayNumber}");
            return JsonSerializer.Deserialize<AIDailyDetailResponse>(response, _jsonOptions)
                ?? throw new Exception($"Failed to parse day {dayNumber} response");
        }

        private async Task<AIDailyDetailResponse> GenerateDailyDetailWithRetryAsync(
            AIGenerationContext context,
            int dayNumber,
            string dayTheme,
            List<string> highlights)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    return await GenerateDailyDetailAsync(context, dayNumber, dayTheme, highlights);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Attempt {Attempt}/{MaxRetries} failed for day {Day}",
                        attempt, MaxRetries, dayNumber);

                    if (attempt == MaxRetries)
                        throw;

                    await Task.Delay(RetryDelayMs * attempt);
                }
            }
            throw new Exception($"All retries failed for day {dayNumber}");
        }

        private async Task<string> CallAIWithRetryAsync(string systemPrompt, string userPrompt, string stepName)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    _logger.LogDebug("AI call [{Step}] attempt {Attempt}", stepName, attempt);

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(systemPrompt),
                        new UserChatMessage(userPrompt)
                    };

                    var options = new ChatCompletionOptions
                    {
                        MaxOutputTokenCount = _aiSettings.MaxTokens,
                        Temperature = (float)_aiSettings.Temperature,
                        ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                    };

                    var completion = await _chatClient.CompleteChatAsync(messages, options);
                    var response = completion.Value.Content[0].Text;

                    if (string.IsNullOrWhiteSpace(response))
                        throw new Exception("Empty AI response");

                    _logger.LogDebug("AI call [{Step}] successful, {Length} chars", stepName, response.Length);
                    return CleanJsonString(response);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI call [{Step}] attempt {Attempt} failed: {Message}",
                        stepName, attempt, ex.Message);

                    if (attempt == MaxRetries)
                        throw;

                    await Task.Delay(RetryDelayMs * attempt);
                }
            }
            throw new Exception($"AI call [{stepName}] failed after {MaxRetries} attempts");
        }

        private string BuildOutlinePrompt(AIGenerationContext context)
        {
            return $@"## THÔNG TIN CHUYẾN ĐI
- Điểm đến: {context.LocationName}
- Xuất phát: {context.CurrentLocationName ?? "Chưa xác định"}
- Số người: {context.NumberOfPeople}
- Thời gian: {context.Duration} ngày
- Ngân sách: {context.PriceRange}
- Sở thích: {string.Join(", ", context.Hobbies)}
- Điểm đến ưu tiên: {string.Join(", ", context.DestinationNames)}
- Ghi chú: {(string.IsNullOrEmpty(context.Notes) ? "Không" : context.Notes)}

## OUTPUT FORMAT
{{
  ""summary"": ""Tóm tắt hấp dẫn về chuyến đi (2-3 câu)"",
  ""totalEstimatedCostFrom"": 0,
  ""totalEstimatedCostTo"": 0,
  ""accommodations"": [
    {{
      ""accommodationType"": ""Hotel/Hostel/Resort/Homestay"",
      ""name"": ""Tên thật"",
      ""address"": ""Địa chỉ đầy đủ"",
      ""pricePerNight"": 0,
      ""description"": ""Review chi tiết"",
      ""amenities"": [""WiFi"", ""Pool""],
      ""rating"": 4.5,
      ""bookingUrl"": null,
      ""contactInfo"": ""SĐT"",
      ""mapUrl"": ""https://www.google.com/maps/search/..."",
      ""imageUrl"": ""https://tse1.mm.bing.net/th?q=...""
    }}
  ],
  ""transportations"": [
    {{
      ""transportationType"": ""Plane/Train/Bus"",
      ""route"": ""A -> B"",
      ""priceFrom"": 0,
      ""priceTo"": 0,
      ""duration"": ""2 tiếng"",
      ""bookingInfo"": ""Hướng dẫn đặt vé"",
      ""tips"": ""Lưu ý"",
      ""provider"": ""Tên hãng""
    }}
  ],
  ""dailyOverview"": [
    {{
      ""dayNumber"": 1,
      ""theme"": ""Chủ đề ngày"",
      ""highlights"": [""Điểm 1"", ""Điểm 2"", ""Điểm 3""]
    }}
  ]
}}

Tạo outline cho {context.Duration} ngày, mỗi ngày có 3 highlights. Chỉ trả JSON thuần túy.";
        }

        private string BuildDailyDetailPrompt(
            AIGenerationContext context,
            int dayNumber,
            string dayTheme,
            List<string> highlights)
        {
            return $@"## THÔNG TIN
- Địa điểm: {context.LocationName}
- Ngân sách: {context.PriceRange}
- Sở thích: {string.Join(", ", context.Hobbies)}
- Ngày: {dayNumber}/{context.Duration}
- Chủ đề: {dayTheme}
- Điểm nhấn: {string.Join(", ", highlights)}

## OUTPUT FORMAT
{{
  ""dayNumber"": {dayNumber},
  ""summary"": ""Tổng quan ngày {dayNumber} (1-2 câu)"",
  ""activities"": [
    {{
      ""name"": ""Tên địa điểm THẬT"",
      ""description"": ""Mô tả CHI TIẾT 30-50 từ: lịch sử, vẻ đẹp, trải nghiệm..."",
      ""startTime"": ""08:00:00"",
      ""endTime"": ""10:00:00"",
      ""destinationName"": ""Tên điểm đến"",
      ""priceFrom"": 0,
      ""priceTo"": 0,
      ""tips"": ""Tips cụ thể từ người bản địa"",
      ""mapUrl"": ""https://www.google.com/maps/search/..."",
      ""imageUrl"": ""https://tse1.mm.bing.net/th?q=..."",
      ""order"": 1
    }}
  ],
  ""foodRecommendations"": [
    {{
      ""mealType"": ""Breakfast/Lunch/Dinner/Snack"",
      ""dishName"": ""Tên món"",
      ""restaurantName"": ""Tên quán THẬT"",
      ""address"": ""Địa chỉ"",
      ""priceFrom"": 0,
      ""priceTo"": 0,
      ""description"": ""Mô tả hương vị"",
      ""specialtyNote"": ""Tại sao món này đặc biệt"",
      ""mapUrl"": ""https://www.google.com/maps/search/..."",
      ""imageUrl"": ""https://tse1.mm.bing.net/th?q=...""
    }}
  ]
}}

BẮT BUỘC: 3 activities và ít nhất 2-3 món ăn. Viết chi tiết, cảm xúc như blog du lịch. Chỉ trả JSON thuần túy.";
        }

        #endregion

        #region Legacy Single-call Generation (Backward Compatibility)

        public async Task<string> GenerateTravelPlanAsync(
            string locationName,
            string? currentLocationName,
            int numberOfPeople,
            int duration,
            string priceRange,
            string notes,
            List<string> hobbies,
            List<string> destinationNames)
        {
            try
            {
                var prompt = BuildLegacyPrompt(
                    locationName, currentLocationName, numberOfPeople,
                    duration, priceRange, notes, hobbies, destinationNames);

                _logger.LogInformation("Generating travel plan with OpenAI for location: {Location}, Model: {Model}",
                    locationName, _aiSettings.Model);

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(@"You are a highly experienced Vietnamese travel expert. 
Your responses must be formatted as valid JSON only (no markdown)."),
                    new UserChatMessage(prompt)
                };

                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = _aiSettings.MaxTokens,
                    Temperature = (float)_aiSettings.Temperature,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                };

                var completion = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions);
                var response = completion.Value.Content[0].Text;

                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("AI service returned empty response");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating travel plan with OpenAI");
                throw;
            }
        }

        private string BuildLegacyPrompt(
            string locationName,
            string? currentLocationName,
            int numberOfPeople,
            int duration,
            string priceRange,
            string notes,
            List<string> hobbies,
            List<string> destinationNames)
        {
            // Keep the original prompt for backward compatibility
            var hobbyList = hobbies.Any() ? string.Join(", ", hobbies) : "Tham quan chung";
            var destinationList = destinationNames.Any() ? string.Join(", ", destinationNames) : "Chưa có gợi ý cụ thể";
            var currentLocation = !string.IsNullOrEmpty(currentLocationName) ? currentLocationName : "Chưa xác định";

            return $@"Tạo kế hoạch du lịch {duration} ngày đến {locationName}.
Số người: {numberOfPeople}, Ngân sách: {priceRange}, Sở thích: {hobbyList}
Điểm đến ưu tiên: {destinationList}, Ghi chú: {notes}

Trả về JSON với format: {{summary, totalEstimatedCostFrom, totalEstimatedCostTo, accommodations[], transportations[], dailyItineraries[]}}";
        }

        #endregion

        private string CleanJsonString(string response)
        {
            var json = response.Trim();
            if (json.StartsWith("```json")) json = json.Substring(7);
            else if (json.StartsWith("```")) json = json.Substring(3);
            if (json.EndsWith("```")) json = json.Substring(0, json.Length - 3);
            return json.Trim();
        }
    }
}
