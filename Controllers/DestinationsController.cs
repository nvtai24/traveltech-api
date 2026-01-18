using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Common;
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
        /// Get all destinations with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDestinations(
            [FromQuery] int? regionId = null,
            [FromQuery] int? locationId = null,
            [FromQuery] string? keyword = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /api/destinations called with regionId: {RegionId}, locationId: {LocationId}, page: {Page}, pageSize: {PageSize}",
         regionId, locationId, page, pageSize);

            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50; // Max page size limit

            var allDestinations = await _destinationService.GetAllDestinationsAsync(regionId, locationId, keyword);
            var destinationsList = allDestinations.ToList();
            var totalCount = destinationsList.Count;

            var pagedDestinations = destinationsList
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var pagedResult = PagedResult<DTOs.DestinationDto>.Create(
                pagedDestinations,
                totalCount,
                page,
                pageSize
            );

            return this.Success(pagedResult, "Destinations retrieved successfully");
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

        /// <summary>
        /// Get destination sharings (user contributions) by destination id
        /// </summary>
        [HttpGet("{id}/sharings")]
        public async Task<IActionResult> GetDestinationSharings(int id)
        {
            _logger.LogInformation("GET /api/destinations/{DestinationId}/sharings called", id);

            // Check if destination exists
            var destination = await _destinationService.GetDestinationByIdAsync(id);
            if (destination == null)
            {
                _logger.LogWarning("Destination not found: {DestinationId}", id);
                return this.NotFound($"Destination with id {id} not found");
            }

            var sharings = await _destinationService.GetDestinationsSharingsAsync(id);
            return this.Success(sharings, "Destination sharings retrieved successfully");
        }
    }
}
