namespace TravelTechApi.DTOs.BlogComment
{
    public class BlogCommentResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int BlogId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<BlogCommentResponse> Replies { get; set; } = new List<BlogCommentResponse>();
    }
}
