using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services.Interfaces;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Controller for Location operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLocations([FromQuery] int? regionId = null)
        {
            _logger.LogInformation("GET /api/locations called with regionId: {RegionId}", regionId);

            var locations = regionId.HasValue
                ? await _locationService.GetLocationsByRegionIdAsync(regionId.Value)
                : await _locationService.GetAllLocationsAsync();

            return this.Success(locations, "Locations retrieved successfully");
        }

        /// <summary>
        /// Get location by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            _logger.LogInformation("GET /api/locations/{LocationId} called", id);
            var location = await _locationService.GetLocationByIdAsync(id);

            if (location == null)
            {
                _logger.LogWarning("Location not found: {LocationId}", id);
                return this.NotFound($"Location with id {id} not found");
            }

            return this.Success(location, "Location retrieved successfully");
        }
    }
}
