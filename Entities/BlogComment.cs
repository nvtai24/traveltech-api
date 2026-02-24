using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelTechApi.Entities
{
    public class BlogComment
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public int BlogId { get; set; }

        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public int? ParentCommentId { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual BlogComment? ParentComment { get; set; }

        public virtual ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
