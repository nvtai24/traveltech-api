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
        public List<string> Images { get; set; } = new List<string>();

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

    }
}