using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.BlogComment;
using TravelTechApi.DTOs.Common;

namespace TravelTechApi.Services.BlogComment
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BlogCommentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BlogCommentResponse> CreateCommentAsync(int blogId, string userId, CreateBlogCommentRequest request)
        {
            // Verify blog exists
            var blogExists = await _context.Blogs.AnyAsync(b => b.Id == blogId);
            if (!blogExists)
                throw new KeyNotFoundException($"Blog with id {blogId} not found");

            // If replying, verify parent comment exists and belongs to the same blog
            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _context.BlogComments
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value && c.BlogId == blogId);

                if (parentComment == null)
                    throw new KeyNotFoundException($"Parent comment with id {request.ParentCommentId.Value} not found in this blog");
            }

            var comment = new Entities.BlogComment
            {
                Content = request.Content,
                BlogId = blogId,
                UserId = userId,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.BlogComments.Add(comment);
            await _context.SaveChangesAsync();

            // Load user for response
            await _context.Entry(comment)
                .Reference(c => c.User)
                .LoadAsync();

            if (comment.User != null)
            {
                // Avatar is now a string property, so we don't need to load it explicitly
            }

            return _mapper.Map<BlogCommentResponse>(comment);
        }

        public async Task<PagedResult<BlogCommentResponse>> GetCommentsByBlogIdAsync(int blogId, int page, int pageSize)
        {
            // Verify blog exists
            var blogExists = await _context.Blogs.AnyAsync(b => b.Id == blogId);
            if (!blogExists)
                throw new KeyNotFoundException($"Blog with id {blogId} not found");

            // Only get root-level comments (no parent)
            var query = _context.BlogComments
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Where(c => c.BlogId == blogId && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<List<BlogCommentResponse>>(items);

            return new PagedResult<BlogCommentResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}
