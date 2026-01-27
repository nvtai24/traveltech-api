using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTechApi.Entities
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public string AuthorId { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; } = null!;

        public string? UpdatedById { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual ApplicationUser? UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsPublished { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
