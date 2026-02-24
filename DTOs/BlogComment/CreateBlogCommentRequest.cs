namespace TravelTechApi.DTOs.BlogComment
{
    public class CreateBlogCommentRequest
    {
        public string Content { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
    }
}
