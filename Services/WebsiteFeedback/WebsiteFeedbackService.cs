using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.WebsiteFeedback;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.WebsiteFeedback;

public class WebsiteFeedbackService : IWebsiteFeedbackService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WebsiteFeedbackService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> CreateFeedbackAsync(WebsiteFeedbackRequest request, string userId)
    {
        var feedback = _mapper.Map<Entities.WebsiteFeedback>(request);
        feedback.UserId = userId;
        feedback.CreatedAt = DateTime.UtcNow;

        _context.WebsiteFeedbacks.Add(feedback);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<List<WebsiteFeedbackResponse>> GetAllFeedbacksAsync()
    {
        return await _context.WebsiteFeedbacks
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt)
            .ProjectTo<WebsiteFeedbackResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
