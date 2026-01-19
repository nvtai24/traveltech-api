using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class DailyItinerary
    {
        [Key]
        public int Id { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        public int DayNumber { get; set; }
        public string Summary { get; set; } = string.Empty;

        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public virtual ICollection<FoodRecommendation> FoodRecommendations { get; set; } = new List<FoodRecommendation>();
    }
}
