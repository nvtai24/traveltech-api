using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Plan;
using TravelTechApi.Services.Interfaces;

namespace TravelTechApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanGenerationService _planService;
        private readonly ILogger<PlansController> _logger;

        public PlansController(
            IPlanGenerationService planService,
            ILogger<PlansController> logger)
        {
            _planService = planService;
            _logger = logger;
        }

        /// <summary>
        /// Generate a new travel plan using AI
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GeneratePlan([FromBody] GeneratePlanRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return this.Unauthorized("User not authenticated");

            var plan = await _planService.GeneratePlanAsync(request, userId);
            return this.Success(plan, "Travel plan generated successfully");
        }

        /// <summary>
        /// Get a specific plan by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return this.Unauthorized("User not authenticated");

            var plan = await _planService.GetPlanByIdAsync(id, userId);
            if (plan == null)
                return this.NotFound("Plan not found");

            return this.Success(plan);
        }

        /// <summary>
        /// Get all plans for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserPlans([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return this.Unauthorized("User not authenticated");

            var plans = await _planService.GetUserPlansAsync(userId, page, pageSize);
            return this.Success(plans);
        }

        /// <summary>
        /// Save a generated plan
        /// </summary>
        [HttpPost("{id}/save")]
        public async Task<IActionResult> SavePlan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return this.Unauthorized("User not authenticated");

            var result = await _planService.SavePlanAsync(id, userId);
            if (!result)
                return this.NotFound("Plan not found");

            return this.Success("Plan saved successfully");
        }

        /// <summary>
        /// Delete a plan
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return this.Unauthorized("User not authenticated");

            var deleted = await _planService.DeletePlanAsync(id, userId);
            if (!deleted)
                return this.NotFound("Plan not found");

            return this.Success("Plan deleted successfully");
        }
    }
}
