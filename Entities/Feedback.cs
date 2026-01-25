
namespace TravelTechApi.Entities;

public class WebsiteFeedback
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public AiPlannerUsefulness AiTripPlannerUsefulness { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool WouldRecommend { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum AiPlannerUsefulness
{
    NotUseful,      // Không
    SlightlyUseful,// Hơi hữu ích
    Useful,        // Hữu ích
    VeryUseful     // Rất hữu ích
}

