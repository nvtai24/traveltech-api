using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Destination
{
    public class UpdateDestinationRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string History { get; set; } = string.Empty;

        [Range(-90, 90)]
        public decimal Lat { get; set; }

        [Range(-180, 180)]
        public decimal Lon { get; set; }

        public string? VideoUrl { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        [Required]
        public int LocationId { get; set; }

        public bool IsVisible { get; set; } = true;

        public List<UpdateFAQRequest>? FAQs { get; set; }
    }
}
