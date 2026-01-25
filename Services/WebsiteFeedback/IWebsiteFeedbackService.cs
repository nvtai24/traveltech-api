using TravelTechApi.DTOs.WebsiteFeedback;

namespace TravelTechApi.Services.WebsiteFeedback;

public interface IWebsiteFeedbackService
{
    Task<bool> CreateFeedbackAsync(WebsiteFeedbackRequest request, string userId);
    Task<List<WebsiteFeedbackResponse>> GetAllFeedbacksAsync();
}
