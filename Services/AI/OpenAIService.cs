using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using TravelTechApi.Common.Settings;

namespace TravelTechApi.Services.AI
{
    public class OpenAIService : IAIService
    {
        private readonly AISettings _aiSettings;
        private readonly ChatClient _chatClient;
        private readonly ILogger<OpenAIService> _logger;



        public OpenAIService(
            IOptions<AISettings> aiSettings,
            ILogger<OpenAIService> logger)
        {
            _aiSettings = aiSettings.Value;
            _logger = logger;

            var apiKey = new ApiKeyCredential(_aiSettings.ApiKey);
            _chatClient = new ChatClient(_aiSettings.Model, apiKey);
        }

        public string GetModel()
        {
            return _aiSettings.Model;
        }

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
                var prompt = BuildPrompt(
                    locationName,
                    currentLocationName,
                    numberOfPeople,
                    duration,
                    priceRange,
                    notes,
                    hobbies,
                    destinationNames
                );

                _logger.LogInformation("Generating travel plan with OpenAI for location: {Location}, Model: {Model}", locationName, _aiSettings.Model);

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("You are an expert travel planner specializing in Vietnam tourism. You create detailed, practical travel itineraries with accurate pricing and recommendations. You MUST respond with valid JSON only, no markdown formatting."),
                    new UserChatMessage(prompt)
                };

                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = _aiSettings.MaxTokens,
                    Temperature = (float)_aiSettings.Temperature,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() // Force JSON response
                };

                _logger.LogInformation("Sending request to OpenAI...");
                var completion = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions);

                var response = completion.Value.Content[0].Text;

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogError("OpenAI returned empty response");
                    throw new Exception("AI service returned empty response");
                }

                _logger.LogInformation("Successfully generated travel plan. Response length: {Length} characters", response.Length);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating travel plan with OpenAI. Model: {Model}, Error: {Message}", _aiSettings.Model, ex.Message);
                throw;
            }
        }

        private string BuildPrompt(
            string locationName,
            string? currentLocationName,
            int numberOfPeople,
            int duration,
            string priceRange,
            string notes,
            List<string> hobbies,
            List<string> destinationNames)
        {
            var hobbyList = hobbies.Any() ? string.Join(", ", hobbies) : "General sightseeing";
            var destinationList = destinationNames.Any() ? string.Join(", ", destinationNames) : "No specific destinations";
            var currentLocation = !string.IsNullOrEmpty(currentLocationName) ? currentLocationName : "Not specified";

            return $@"
Bạn là một chuyên gia lập kế hoạch du lịch tại Việt Nam. Hãy tạo một kế hoạch du lịch chi tiết với thông tin sau:

**Thông tin chuyến đi:**
- Địa điểm đến: {locationName}
- Điểm xuất phát: {currentLocation}
- Số người: {numberOfPeople}
- Mức giá: {priceRange}
- Sở thích: {hobbyList}
- Các điểm đến gợi ý: {destinationList}
- Ghi chú thêm: {(string.IsNullOrEmpty(notes) ? "Không có" : notes)}

**Yêu cầu:**
1. Đề xuất 1-2 lựa chọn lưu trú phù hợp với mức giá
2. Đề xuất phương tiện di chuyển từ điểm xuất phát đến địa điểm (nếu có thông tin)
3. Lập kế hoạch chi tiết cho từng ngày bao gồm:
   - Các hoạt động/địa điểm tham quan (ưu tiên các điểm đến gợi ý nếu có)
   - Thời gian cho mỗi hoạt động
   - Gợi ý món ăn cho bữa sáng, trưa, tối
   - Khoảng giá cho mỗi hoạt động và món ăn

**Format JSON trả về:**
{{
  ""summary"": ""Tóm tắt chuyến đi"",
  ""totalEstimatedCostFrom"": 0,
  ""totalEstimatedCostTo"": 0,
  ""accommodations"": [
    {{
      ""accommodationType"": ""Hotel/Hostel/Resort/Homestay"",
      ""name"": ""Tên nơi lưu trú"",
      ""address"": ""Địa chỉ"",
      ""pricePerNight"": 0,
      ""description"": ""Mô tả"",
      ""amenities"": [""WiFi"", ""Pool"", ""Breakfast""],
      ""rating"": 4.5,
      ""bookingUrl"": null,
      ""contactInfo"": ""Thông tin liên hệ"",
      ""mapUrl"": ""Link Google Maps tìm kiếm khách sạn (https://www.google.com/maps/search/?api=1&query=...)"",
      ""imageUrl"": null
    }}
  ],
  ""transportations"": [
    {{
      ""transportationType"": ""Plane/Train/Bus/Taxi/Motorbike"",
      ""route"": ""Điểm A -> Điểm B"",
      ""priceFrom"": 0,
      ""priceTo"": 0,
      ""duration"": ""2 giờ"",
      ""bookingInfo"": ""Thông tin đặt vé"",
      ""tips"": ""Lời khuyên"",
      ""provider"": ""Nhà cung cấp""
    }}
  ],
  ""dailyItineraries"": [
    {{
      ""dayNumber"": 1,
      ""summary"": ""Tóm tắt ngày 1"",
      ""activities"": [
        {{
          ""name"": ""Tên hoạt động"",
          ""description"": ""Mô tả chi tiết"",
          ""startTime"": ""09:00:00"",
          ""endTime"": ""11:00:00"",
          ""destinationName"": ""Tên điểm đến (nếu có)"",
          ""priceFrom"": 0,
          ""priceTo"": 0,
          ""tips"": ""Lời khuyên"",
          ""mapUrl"": ""Link Google Maps tìm kiếm địa điểm"",
          ""order"": 1
        }}
      ],
      ""foodRecommendations"": [
        {{
          ""mealType"": ""Breakfast/Lunch/Dinner/Snack"",
          ""dishName"": ""Tên món ăn"",
          ""restaurantName"": ""Tên nhà hàng"",
          ""address"": ""Địa chỉ"",
          ""priceFrom"": 0,
          ""priceTo"": 0,
          ""description"": ""Mô tả món ăn"",
          ""specialtyNote"": ""Đặc sản địa phương"",
          ""mapUrl"": ""Link Google Maps tìm kiếm nhà hàng/quán ăn"",
          ""imageUrl"": null
        }}
      ]
    }}
  ]
}}

**Lưu ý quan trọng:**
- Trả về CHÍNH XÁC theo format JSON trên, không thêm markdown hay text khác
- Giá cả phải thực tế và phù hợp với mức giá đã chọn
- Ưu tiên các điểm đến đã gợi ý trong danh sách destinations
- Nếu có điểm xuất phát, tính toán chi phí và thời gian di chuyển
";
        }
    }
}
