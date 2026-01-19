using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.DTOs.Plan
{
    public class PlanResponse
    {
        public int Id { get; set; }
        public LocationResponse Location { get; set; } = null!;
        public LocationResponse? CurrentLocation { get; set; }
        public int NumberOfPeople { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCostEstimatedFrom { get; set; }
        public decimal TotalCostEstimatedTo { get; set; }
        public string PriceSetting { get; set; } = string.Empty;
        public List<string> Hobbies { get; set; } = new();
        public string Note { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? GeneratedAt { get; set; }
        public string? AIModel { get; set; }

        public List<AccommodationRecommendationResponse> Accommodations { get; set; } = new();
        public List<TransportationRecommendationResponse> Transportations { get; set; } = new();
        public List<DailyItineraryResponse> DailyItineraries { get; set; } = new();
    }
}
