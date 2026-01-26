namespace TravelTechApi.DTOs.AI
{
    /// <summary>
    /// Combined result from multi-step AI generation
    /// </summary>
    public class AIFullPlanResponse
    {
        public string Summary { get; set; } = string.Empty;
        public decimal TotalEstimatedCostFrom { get; set; }
        public decimal TotalEstimatedCostTo { get; set; }
        public List<AIAccommodationDto> Accommodations { get; set; } = new();
        public List<AITransportationDto> Transportations { get; set; } = new();
        public List<AIDailyDetailResponse> DailyItineraries { get; set; } = new();
    }

    /// <summary>
    /// Input context for AI generation
    /// </summary>
    public class AIGenerationContext
    {
        public string LocationName { get; set; } = string.Empty;
        public string? CurrentLocationName { get; set; }
        public int NumberOfPeople { get; set; }
        public int Duration { get; set; }
        public string PriceRange { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<string> Hobbies { get; set; } = new();
        public List<string> DestinationNames { get; set; } = new();
    }
}
