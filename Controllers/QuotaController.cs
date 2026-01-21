using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services.Quota;
using TravelTechApi.Services.UserPlanSubscription;

namespace TravelTechApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotaController : ControllerBase
{
    private readonly IQuotaService _quotaService;
    private readonly IUserPlanSubscriptionService _userPlanSubscriptionService;

    public QuotaController(IQuotaService quotaService, IUserPlanSubscriptionService userPlanSubscriptionService)
    {
        _quotaService = quotaService;
        _userPlanSubscriptionService = userPlanSubscriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuota()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return this.Unauthorized("User not authenticated");

        var plan = await _userPlanSubscriptionService.GetCurrentPlanAsync(userId);
        var limit = plan.DailyLimit;

        var quota = await _quotaService.CheckLimit("ai_plan_generation", userId, limit); // Hardcode
        return this.Success(quota, "Quota retrieved successfully");
    }
}
