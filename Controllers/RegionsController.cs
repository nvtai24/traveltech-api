using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Services.Interfaces;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Controller for Region operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(IRegionService regionService, ILogger<RegionsController> logger)
        {
            _regionService = regionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all regions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            _logger.LogInformation("GET /api/regions called");
            var regions = await _regionService.GetAllRegionsAsync();
            return this.Success(regions, "Regions retrieved successfully");
        }
    }
}
