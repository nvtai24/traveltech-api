using TravelTechApi.Entities;

namespace TravelTechApi.DTOs.WebsiteFeedback;

public class WebsiteFeedbackResponse
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public AiPlannerUsefulness AiTripPlannerUsefulness { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool WouldRecommend { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}