using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Blog;
using TravelTechApi.DTOs.BlogComment;
using TravelTechApi.Services.Auth;
using TravelTechApi.Services.Blog;
using TravelTechApi.Services.BlogComment;

namespace TravelTechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCommentService _blogCommentService;
        private readonly ICurrentUserService _currentUserService;

        public BlogsController(IBlogService blogService, IBlogCommentService blogCommentService, ICurrentUserService currentUserService)
        {
            _blogService = blogService;
            _blogCommentService = blogCommentService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Create a new blog (Admin/Moderator)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
        {
            var userId = _currentUserService.UserId;
            var result = await _blogService.CreateBlogAsync(userId!, request);
            return this.Created("Blog created successfully", result);
        }

        /// <summary>
        /// Update an existing blog (Admin/Moderator)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] UpdateBlogRequest request)
        {
            var userId = _currentUserService.UserId;
            var result = await _blogService.UpdateBlogAsync(id, userId!, request);
            return Ok(result);
        }

        /// <summary>
        /// Delete a blog (Admin/Moderator)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var userId = _currentUserService.UserId;
            await _blogService.DeleteBlogAsync(id, userId!);
            return this.Success("Blog deleted successfully");
        }

        /// <summary>
        /// Get blog by id (Public)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var result = await _blogService.GetBlogByIdPublicAsync(id);
            if (result == null)
            {
                return this.NotFound("Blog not found");
            }
            return this.Success(result, "Blog fetched successfully");
        }

        /// <summary>
        /// Get blog by id (Admin/Owner)
        /// </summary>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> GetBlogByIdAdmin(int id)
        {
            var result = await _blogService.GetBlogByIdAdminAsync(id);
            if (result == null)
            {
                return this.NotFound("Blog not found");
            }
            return this.Success(result, "Blog fetched successfully");
        }

        /// <summary>
        /// Get all blogs (Public - Published only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _blogService.GetAllBlogsPublicAsync(page, pageSize, searchTerm);
            return this.Success(result, "Blogs fetched successfully");
        }

        /// <summary>
        /// Get all blogs (Admin - All)
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> GetAllBlogsAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isPublished = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _blogService.GetAllBlogsAdminAsync(page, pageSize, searchTerm, isPublished);
            return this.Success(result, "Blogs fetched successfully");
        }

        /// <summary>
        /// Add a comment to a blog (Authenticated users)
        /// </summary>
        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(int id, [FromBody] CreateBlogCommentRequest request)
        {
            var userId = _currentUserService.UserId;
            var result = await _blogCommentService.CreateCommentAsync(id, userId!, request);
            return this.Created("Comment created successfully", result);
        }

        /// <summary>
        /// Get comments for a blog (Public)
        /// </summary>
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _blogCommentService.GetCommentsByBlogIdAsync(id, page, pageSize);
            return this.Success(result, "Comments fetched successfully");
        }
    }
}
