using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Blog
{
    public class UpdateBlogRequest
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public bool IsPublished { get; set; }
    }
}
