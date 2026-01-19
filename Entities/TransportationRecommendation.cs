using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class TransportationRecommendation
    {
        [Key]
        public int Id { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        // AI will suggest transportation type based on budget (not FK)
        public string TransportationType { get; set; } = string.Empty; // "Plane", "Train", "Bus", "Taxi", "Motorbike", etc.
        public string Route { get; set; } = string.Empty; // "Hanoi -> Da Nang"

        // Price range
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }

        public string Duration { get; set; } = string.Empty; // "2 hours"
        public string? BookingInfo { get; set; }
        public string? Tips { get; set; }
        public string? Provider { get; set; } // "Vietnam Airlines", "Grab", etc.
    }
}
