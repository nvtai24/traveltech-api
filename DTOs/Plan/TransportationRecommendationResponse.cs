namespace TravelTechApi.DTOs.Plan
{
    public class TransportationRecommendationResponse
    {
        public int Id { get; set; }
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
