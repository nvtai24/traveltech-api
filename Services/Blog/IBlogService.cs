using TravelTechApi.DTOs.Blog;
using TravelTechApi.DTOs.Common;

namespace TravelTechApi.Services.Blog
{
    public interface IBlogService
    {
        Task<BlogAdminResponse> CreateBlogAsync(string authorId, CreateBlogRequest request);
        Task<BlogAdminResponse> UpdateBlogAsync(int id, string authorId, UpdateBlogRequest request);
        Task DeleteBlogAsync(int id, string authorId);
        Task<BlogPublicResponse?> GetBlogByIdPublicAsync(int id);
        Task<BlogAdminResponse?> GetBlogByIdAdminAsync(int id);
        Task<PagedResult<BlogPublicResponse>> GetAllBlogsPublicAsync(int page, int pageSize, string? searchTerm);
        Task<PagedResult<BlogAdminResponse>> GetAllBlogsAdminAsync(int page, int pageSize, string? searchTerm, bool? isPublished);
    }
}
