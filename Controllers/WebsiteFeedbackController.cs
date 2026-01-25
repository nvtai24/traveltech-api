using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.WebsiteFeedback;
using TravelTechApi.Services.WebsiteFeedback;

namespace TravelTechApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebsiteFeedbackController : ControllerBase
{
    private readonly IWebsiteFeedbackService _feedbackService;

    public WebsiteFeedbackController(IWebsiteFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Submit website feedback (authenticated users only)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFeedbackAsync([FromBody] WebsiteFeedbackRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized("User not authenticated");
        }

        var result = await _feedbackService.CreateFeedbackAsync(request, userId);
        if (result)
        {
            return this.Success("Feedback submitted successfully");
        }

        return this.InternalServerError("Failed to submit feedback");
    }

    /// <summary>
    /// Get all website feedback (admin only)
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllFeedbacksAsync()
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
        return this.Success(feedbacks);
    }
}
