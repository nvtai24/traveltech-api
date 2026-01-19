namespace TravelTechApi.DTOs.Plan
{
    public class AccommodationRecommendationResponse
    {
        public int Id { get; set; }
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
}
