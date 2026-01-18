using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class DestinationSharing
    {
        [Key]
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int DestinationId { get; set; }
        public virtual Destination Destination { get; set; } = null!;
        public virtual ICollection<CloudinaryFileInfo> Images { get; set; } = new List<CloudinaryFileInfo>();

    }
}