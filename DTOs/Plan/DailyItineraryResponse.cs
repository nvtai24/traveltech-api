namespace TravelTechApi.DTOs.Plan
{
    public class DailyItineraryResponse
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<ActivityResponse> Activities { get; set; } = new();
        public List<FoodRecommendationResponse> FoodRecommendations { get; set; } = new();
    }
}
