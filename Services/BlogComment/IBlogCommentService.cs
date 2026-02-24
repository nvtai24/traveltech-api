using TravelTechApi.DTOs.BlogComment;
using TravelTechApi.DTOs.Common;

namespace TravelTechApi.Services.BlogComment
{
    public interface IBlogCommentService
    {
        Task<BlogCommentResponse> CreateCommentAsync(int blogId, string userId, CreateBlogCommentRequest request);
        Task<PagedResult<BlogCommentResponse>> GetCommentsByBlogIdAsync(int blogId, int page, int pageSize);
    }
}
