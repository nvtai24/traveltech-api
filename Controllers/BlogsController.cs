using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Blog;
using TravelTechApi.Services.Auth;
using TravelTechApi.Services.Blog;

namespace TravelTechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ICurrentUserService _currentUserService;

        public BlogsController(IBlogService blogService, ICurrentUserService currentUserService)
        {
            _blogService = blogService;
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
            return CreatedAtAction(nameof(GetBlogByIdAdmin), new { id = result.Id }, result);
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
            return NoContent();
        }

        /// <summary>
        /// Get blog by id (Public)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var result = await _blogService.GetBlogByIdPublicAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Get blog by id (Admin/Owner)
        /// </summary>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Moderator)]
        public async Task<IActionResult> GetBlogByIdAdmin(int id)
        {
            var result = await _blogService.GetBlogByIdAdminAsync(id);
            return Ok(result);
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
            return Ok(result);
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
            return Ok(result);
        }
    }
}
