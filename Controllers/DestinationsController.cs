using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Services;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Controller for Destination operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
        private readonly IDestinationService _destinationService;
        private readonly ILogger<DestinationsController> _logger;

        public DestinationsController(IDestinationService destinationService, ILogger<DestinationsController> logger)
        {
            _destinationService = destinationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all destinations
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDestinations([FromQuery] int? locationId = null)
        {
            _logger.LogInformation("GET /api/destinations called with locationId: {LocationId}", locationId);

            var destinations = locationId.HasValue
                ? await _destinationService.GetDestinationsByLocationIdAsync(locationId.Value)
                : await _destinationService.GetAllDestinationsAsync();

            return this.Success(destinations, "Destinations retrieved successfully");
        }

        /// <summary>
        /// Get destination by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDestinationById(int id)
        {
            _logger.LogInformation("GET /api/destinations/{DestinationId} called", id);
            var destination = await _destinationService.GetDestinationByIdAsync(id);

            if (destination == null)
            {
                _logger.LogWarning("Destination not found: {DestinationId}", id);
                return this.NotFound($"Destination with id {id} not found");
            }

            return this.Success(destination, "Destination retrieved successfully");
        }
    }
}
