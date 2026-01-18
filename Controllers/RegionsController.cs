using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services;

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

        /// <summary>
        /// Get region by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegionById(int id)
        {
            _logger.LogInformation("GET /api/regions/{RegionId} called", id);
            var region = await _regionService.GetRegionByIdAsync(id);

            if (region == null)
            {
                _logger.LogWarning("Region not found: {RegionId}", id);
                return this.NotFound($"Region with id {id} not found");
            }

            return this.Success(region, "Region retrieved successfully");
        }
    }
}
