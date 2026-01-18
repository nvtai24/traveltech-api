using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public virtual Region Region { get; set; } = null!;
        public virtual ICollection<Destination> Destinations { get; set; } = new List<Destination>();
    }
}