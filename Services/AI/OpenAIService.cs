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
                    new SystemChatMessage(@"You are a highly experienced Vietnamese travel expert and local guide with deep knowledge of:
- Real accommodation options and their actual pricing across all price ranges
- Authentic local restaurants, street food vendors, and dining experiences
- Practical transportation routes, costs, and booking methods
- Hidden gems and popular tourist attractions with realistic time estimates
- Cultural insights and practical travel tips for Vietnam

Your responses must be:
✓ Based on REAL places with accurate names and locations
✓ Priced realistically according to current Vietnam market rates
✓ Practical and achievable within the given timeframe
✓ Culturally appropriate and locally relevant
✓ Formatted as valid JSON only (no markdown, no code blocks, no extra text)
✓ **Enriched with valid image URLs** for locations and foods whenever possible

Quality Standards:
- Accommodations: Suggest real hotels/hostels/homestays with accurate price ranges
- Food: Recommend actual local dishes and restaurants appropriate to the location
- Activities: Provide realistic time allocations (e.g., 2-3 hours for museums, 1 hour for meals)
- Pricing: Match the user's budget level (Rẻ: <500k/day, Trung bình: 500k-1.5M/day, Cao: >1.5M/day)
- Maps: Generate proper Google Maps search URLs for all locations
- Images: MUST use the Bing Thumbnail format provided in requirements. Do NOT invent other URLs."),
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
            var hobbyList = hobbies.Any() ? string.Join(", ", hobbies) : "Tham quan chung";
            var destinationList = destinationNames.Any() ? string.Join(", ", destinationNames) : "Chưa có gợi ý cụ thể";
            var currentLocation = !string.IsNullOrEmpty(currentLocationName) ? currentLocationName : "Chưa xác định";

            // Build structured prompt with clear sections
            var contextSection = $@"## THÔNG TIN CHUYẾN ĐI
📍 Điểm đến: {locationName}
🚀 Xuất phát từ: {currentLocation}
👥 Số người: {numberOfPeople} người
📅 Thời gian: {duration} ngày
💰 Ngân sách: {priceRange}
🎯 Sở thích: {hobbyList}
⭐ Điểm đến ưu tiên: {destinationList}
📝 Ghi chú: {(string.IsNullOrEmpty(notes) ? "Không có" : notes)}";

            var requirementsSection = @"## YÊU CẦU CHI TIẾT

### 1. Lưu trú (1-2 lựa chọn)
- Đề xuất khách sạn/nhà nghỉ/homestay THỰC TẾ với tên và địa chỉ cụ thể
- Giá phải phù hợp với mức ngân sách đã chọn
- Bao gồm tiện nghi, đánh giá, và link Google Maps
- **HÌNH ẢNH**: BẮT BUỘC sử dụng format sau để tạo link ảnh: `https://tse1.mm.bing.net/th?q={Tên Khách Sạn} + {Thành Phố}`

### 2. Di chuyển
- Phương tiện từ điểm xuất phát đến điểm đến (nếu có thông tin)
- Chi phí thực tế, thời gian di chuyển, nhà cung cấp
- Lời khuyên đặt vé và tips hữu ích

### 3. Lịch trình từng ngày
Mỗi ngày bao gồm:
- **Hoạt động**: Địa điểm THỰC TẾ, thời gian hợp lý.
- **Mô tả**: Viết sâu sắc (2-3 câu), nêu bật TẠI SAO nên đến đây, không viết chung chung kiểu ""đẹp lắm"".
- **Tips**: Mẹo của người bản địa (góc chụp ảnh, giờ đi tránh đông, lưu ý trang phục...).
- **Ẩm thực**: Món ăn ĐẶC TRƯNG, review ngắn gọn về hương vị.
- **Giá cả**: Ước tính chính xác.
- **Maps**: Link Google Maps chuẩn.
- **HÌNH ẢNH**: BẮT BUỘC sử dụng format sau:
  - Cho địa điểm (Activities): `https://tse1.mm.bing.net/th?q={Từ khóa Hoạt động} + {Tên Địa Điểm} + {Thành Phố}`
  - Cho món ăn (Food): `https://tse1.mm.bing.net/th?q={Tên Món Ăn} + {Tên Quán}`";

            var constraintsSection = $@"## RÀNG BUỘC QUAN TRỌNG

✓ Ưu tiên các điểm đến đã gợi ý: {destinationList}
✓ Thời gian hoạt động phải THỰC TẾ
✓ Giá cả phải phù hợp mức '{priceRange}':
  - Rẻ: <500,000 VNĐ/ngày/người
  - Trung bình: 500,000-1,500,000 VNĐ/ngày/người  
  - Cao: >1,500,000 VNĐ/ngày/người
✓ Tên địa điểm phải là tên THẬT
✓ **Nội dung**: KHÔNG VIẾT NGẮN GỌN. Hãy viết như một bài blog du lịch nhỏ, đầy cảm hứng.
✓ **Image URLs**: Format Bing Thumbnail như yêu cầu.";

            var jsonFormatSection = @"## OUTPUT FORMAT (JSON ONLY)

Trả về CHÍNH XÁC theo cấu trúc sau, KHÔNG thêm markdown:

{
  ""summary"": ""Tóm tắt hấp dẫn về chuyến đi (2-3 câu)"",
  ""totalEstimatedCostFrom"": 0,
  ""totalEstimatedCostTo"": 0,
  ""accommodations"": [
    {
      ""accommodationType"": ""Hotel/Hostel/Resort/Homestay"",
      ""name"": ""Tên thật"",
      ""address"": ""Địa chỉ đầy đủ"",
      ""pricePerNight"": 0,
      ""description"": ""Review chi tiết về không gian, vị trí, điểm cộng/trừ"",
      ""amenities"": [""WiFi"", ""Pool"", ""Breakfast""],
      ""rating"": 4.5,
      ""bookingUrl"": null,
      ""contactInfo"": ""SĐT/Web"",
      ""mapUrl"": ""url"",
      ""imageUrl"": null
    }
  ],
  ""transportations"": [
    {
      ""transportationType"": ""Plane/Train/Bus/Taxi"",
      ""route"": ""A -> B"",
      ""priceFrom"": 0,
      ""priceTo"": 0,
      ""duration"": ""time"",
      ""bookingInfo"": ""Hướng dẫn đặt vé chi tiết"",
      ""tips"": ""Lưu ý khi di chuyển"",
      ""provider"": ""Hãng xe/bay""
    }
  ],
  ""dailyItineraries"": [
    {
      ""dayNumber"": 1,
      ""summary"": ""Tổng quan trải nghiệm ngày 1"",
      ""activities"": [
        {
          ""name"": ""Tên địa điểm"",
          ""description"": ""Mô tả chi tiết (30-50 từ): Lịch sử, vẻ đẹp, trải nghiệm nên thử..."",
          ""startTime"": ""HH:mm:ss"",
          ""endTime"": ""HH:mm:ss"",
          ""destinationName"": ""Tên điểm đến"",
          ""priceFrom"": 0,
          ""priceTo"": 0,
          ""tips"": ""Lời khuyên cụ thể (Góc check-in, món nên gọi...)"",
          ""mapUrl"": ""url"",
          ""imageUrl"": null,
          ""order"": 1
        }
      ],
      ""foodRecommendations"": [
        {
          ""mealType"": ""Lunch"",
          ""dishName"": ""Tên món"",
          ""restaurantName"": ""Tên quán"",
          ""address"": ""Địa chỉ"",
          ""priceFrom"": 0,
          ""priceTo"": 0,
          ""description"": ""Mô tả hương vị món ăn"",
          ""specialtyNote"": ""Tại sao món này đặc biệt?"",
          ""mapUrl"": ""url"",
          ""imageUrl"": null
        }
      ]
    }
  ]
}
";

            var exampleSection = @"## VÍ DỤ THAM KHẢO

Ví dụ (Đà Lạt):

{
  ""summary"": ""Hành trình 2 ngày khám phá Đà Lạt mộng mơ, từ những đồi thông reo trong gió đến hương vị cà phê phố núi đặc trưng."",
  ""dailyItineraries"": [
    {
      ""dayNumber"": 1,
      ""activities"": [
        {
          ""name"": ""Quảng trường Lâm Viên"",
          ""description"": ""Biểu tượng của Đà Lạt với nụ hoa Atiso khổng lồ bằng kính rực rỡ dưới nắng. Nơi đây không chỉ là điểm check-in 'quốc dân' mà còn có không gian rộng lớn nhìn ra Hồ Xuân Hương thơ mộng, rất thích hợp để dạo bộ và hít thở không khí trong lành."",
          ""startTime"": ""08:00:00"",
          ""endTime"": ""09:30:00"",
          ""tips"": ""Nên đi vào sáng sớm để tránh đông đúc và nắng gắt. Dưới hầm quảng trường có siêu thị Big C và rạp phim."",
          ""mapUrl"": ""https://google.com..."",
          ""imageUrl"": null
        }
      ],
      ""foodRecommendations"": [
        {
          ""mealType"": ""Breakfast"",
            ""dishName"": ""Bánh mì xíu mại"",
          ""restaurantName"": ""Bánh mì xíu mại Hoàng Diệu"",
          ""description"": ""Viên xíu mại mềm thơm, nước dùng ngọt thanh từ xương hầm, chấm cùng bánh mì giòn rụm tạo nên bữa sáng ấm bụng giữa tiết trời se lạnh."",
          ""specialtyNote"": ""Nhớ gọi thêm ly sữa đậu nành nóng để trọn vẹn combo bữa sáng Đà Lạt."",
          ""mapUrl"": ""https://google.com...""
        }
      ]
    }
  ]
}

Hãy viết response với độ chi tiết và giọng văn cảm xúc tương tự.";

            return $@"{contextSection}

{requirementsSection}

{constraintsSection}

{jsonFormatSection}

{exampleSection}";
        }
    }
}
