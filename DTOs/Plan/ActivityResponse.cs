namespace TravelTechApi.DTOs.Plan
{
    public class ActivityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        // TODO: Add when implementing Destination linking
        // public DestinationResponse? Destination { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Tips { get; set; }
        public string? MapUrl { get; set; }
        public string? ImageUrl { get; set; }
        public int Order { get; set; }
    }
}
