namespace TravelTechApi.DTOs.Blog
{
    public class BlogPublicResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
