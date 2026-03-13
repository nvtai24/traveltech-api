using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class Destination
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string? VideoUrl { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public int LocationId { get; set; }
        public virtual Location Location { get; set; } = null!;
        public List<string> Images { get; set; } = new List<string>();
        public virtual ICollection<FAQ> FAQs { get; set; } = new List<FAQ>();
        public virtual ICollection<DestinationSharing> Sharings { get; set; } = new List<DestinationSharing>();
        public bool IsVisible { get; set; } = true;
    }
}