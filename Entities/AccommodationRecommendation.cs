using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class AccommodationRecommendation
    {
        [Key]
        public int Id { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        // AI will suggest accommodation type based on budget (not FK)
        public string AccommodationType { get; set; } = string.Empty; // "Hotel", "Hostel", "Resort", "Homestay", etc.
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new List<string>(); // ["WiFi", "Pool", "Breakfast", etc.]
        public decimal? Rating { get; set; }
        public string? BookingUrl { get; set; }
        public string? ContactInfo { get; set; }
        public string? ImageUrl { get; set; }
        public string? MapUrl { get; set; }
    }
}
