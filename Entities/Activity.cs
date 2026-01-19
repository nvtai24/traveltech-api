using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        public int DailyItineraryId { get; set; }
        public DailyItinerary DailyItinerary { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // TODO: Link to existing Destination - implement later
        // Optional link to existing Destination
        // public int? DestinationId { get; set; }
        // public Destination? Destination { get; set; }

        // Price range
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public string? Tips { get; set; }
        public string? MapUrl { get; set; }
        public int Order { get; set; } // Order of activities in the day
    }
}
