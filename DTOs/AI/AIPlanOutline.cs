namespace TravelTechApi.DTOs.AI
{
    /// <summary>
    /// Response DTOs for Step 1: Plan Outline Generation
    /// </summary>
    public class AIPlanOutlineResponse
    {
        public string Summary { get; set; } = string.Empty;
        public decimal TotalEstimatedCostFrom { get; set; }
        public decimal TotalEstimatedCostTo { get; set; }
        public List<AIAccommodationDto> Accommodations { get; set; } = new();
        public List<AITransportationDto> Transportations { get; set; } = new();
        public List<AIDailyOverviewDto> DailyOverview { get; set; } = new();
    }

    public class AIDailyOverviewDto
    {
        public int DayNumber { get; set; }
        public string Theme { get; set; } = string.Empty;
        public List<string> Highlights { get; set; } = new();
    }

    public class AIAccommodationDto
    {
        public string AccommodationType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public decimal? Rating { get; set; }
        public string? BookingUrl { get; set; }
        public string? ContactInfo { get; set; }
        public string? ImageUrl { get; set; }
        public string? MapUrl { get; set; }
    }

    public class AITransportationDto
    {
        public string TransportationType { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string? BookingInfo { get; set; }
        public string? Tips { get; set; }
        public string? Provider { get; set; }
    }
}
