using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Blog;
using TravelTechApi.DTOs.Common;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Blog
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BlogService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BlogAdminResponse> CreateBlogAsync(string authorId, CreateBlogRequest request)
        {
            var blog = new Entities.Blog
            {
                Title = request.Title,
                Content = request.Content,
                ThumbnailUrl = request.ThumbnailUrl,
                AuthorId = authorId,
                Tags = request.Tags,
                IsPublished = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            await LoadAuthorAsync(blog);
            return _mapper.Map<BlogAdminResponse>(blog);
        }

        public async Task<BlogAdminResponse> UpdateBlogAsync(int id, string authorId, UpdateBlogRequest request)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
                throw new KeyNotFoundException($"Blog with id {id} not found");

            // Since only Admin/Moderator can access this, we remove the author check
            // if (blog.AuthorId != authorId)
            //    throw new UnauthorizedAccessException("You are not authorized to update this blog");

            // blog.AuthorId check removed for Admin/Moderator

            _mapper.Map(request, blog);

            blog.UpdatedAt = DateTime.UtcNow;
            blog.UpdatedById = authorId; // Use authorId here as it represents the current user performing the update

            await _context.SaveChangesAsync();

            await LoadAuthorAsync(blog);
            return _mapper.Map<BlogAdminResponse>(blog);
        }

        public async Task DeleteBlogAsync(int id, string authorId)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
                throw new KeyNotFoundException($"Blog with id {id} not found");

            // Since only Admin/Moderator can access this, we remove the author check
            // if (blog.AuthorId != authorId)
            //    throw new UnauthorizedAccessException("You are not authorized to delete this blog");

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }

        public async Task<BlogPublicResponse> GetBlogByIdPublicAsync(int id)
        {
            var blog = await _context.Blogs
               .Include(b => b.Author)
               .ThenInclude(u => u.Avatar)
               .FirstOrDefaultAsync(b => b.Id == id && b.IsPublished);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with id {id} not found");

            return _mapper.Map<BlogPublicResponse>(blog);
        }

        public async Task<BlogAdminResponse> GetBlogByIdAdminAsync(int id)
        {
            var blog = await _context.Blogs
               .Include(b => b.Author)
               .ThenInclude(u => u.Avatar)
               .Include(b => b.UpdatedBy)
               .ThenInclude(u => u.Avatar)
               .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with id {id} not found");

            return _mapper.Map<BlogAdminResponse>(blog);
        }

        public async Task<PagedResult<BlogPublicResponse>> GetAllBlogsPublicAsync(int page, int pageSize, string? searchTerm)
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .ThenInclude(u => u.Avatar)
                .Where(b => b.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         b.Tags.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            query = query.OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<List<BlogPublicResponse>>(items);

            return new PagedResult<BlogPublicResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PagedResult<BlogAdminResponse>> GetAllBlogsAdminAsync(int page, int pageSize, string? searchTerm, bool? isPublished)
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .ThenInclude(u => u.Avatar)
                .Include(b => b.UpdatedBy)
                .ThenInclude(u => u.Avatar)
                .AsQueryable();

            if (isPublished.HasValue)
            {
                query = query.Where(b => b.IsPublished == isPublished.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         b.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         b.Tags.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            query = query.OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<List<BlogAdminResponse>>(items);

            return new PagedResult<BlogAdminResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        private async Task LoadAuthorAsync(Entities.Blog blog)
        {
            // If author is not loaded (e.g. after create), load it
            if (blog.Author == null)
            {
                await _context.Entry(blog)
                   .Reference(b => b.Author)
                   .LoadAsync();

                if (blog.Author != null)
                {
                    await _context.Entry(blog.Author)
                      .Reference(u => u.Avatar)
                      .LoadAsync();
                }
            }

            // Load UpdatedBy if present
            if (blog.UpdatedById != null && blog.UpdatedBy == null)
            {
                await _context.Entry(blog)
                   .Reference(b => b.UpdatedBy)
                   .LoadAsync();

                if (blog.UpdatedBy != null)
                {
                    await _context.Entry(blog.UpdatedBy)
                      .Reference(u => u.Avatar)
                      .LoadAsync();
                }
            }
        }
    }
}
