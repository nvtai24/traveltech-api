using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTechApi.Entities
{
    public class Plan
    {
        [Key]
        public int Id { get; set; }

        // Destination
        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        // Current location (optional - for transportation cost calculation)
        public int? CurrentLocationId { get; set; }
        public Location? CurrentLocation { get; set; }

        // Trip details
        public int NumberOfPeople { get; set; }
        public int Duration { get; set; }
        public decimal TotalCostEstimatedFrom { get; set; }
        public decimal TotalCostEstimatedTo { get; set; }

        [Column(TypeName = "jsonb")]
        public string? AIResponseJson { get; set; }

        // Preferences
        public ICollection<TravelHobby> Hobbies { get; set; } = new List<TravelHobby>();
        public int PriceSettingId { get; set; }
        public PriceSetting PriceSetting { get; set; } = null!;
        public string Note { get; set; } = string.Empty;

        // Plan status and metadata
        public bool IsSaved { get; set; } = false;
        public DateTime? GeneratedAt { get; set; }
        public string? AIModel { get; set; }

        // User
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Navigation properties for generated plan
        public virtual ICollection<DailyItinerary> DailyItineraries { get; set; } = new List<DailyItinerary>();
        public virtual ICollection<AccommodationRecommendation> AccommodationRecommendations { get; set; } = new List<AccommodationRecommendation>();
        public virtual ICollection<TransportationRecommendation> TransportationRecommendations { get; set; } = new List<TransportationRecommendation>();
    }
}