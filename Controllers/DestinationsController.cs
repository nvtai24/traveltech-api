using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Services.Interfaces;

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

            var pagedResult = PagedResult<DestinationResponse>.Create(
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

        /// <summary>
        /// Create a new destination sharing (user contribution)
        /// </summary>
        [HttpPost("{id}/sharings")]
        [Authorize(Roles = AppRoles.User)] // Require authentication
        public async Task<IActionResult> CreateDestinationSharing(int id, [FromForm] CreateDestinationSharingRequest dto)
        {
            try
            {
                // Get user ID from JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token");
                    return this.Unauthorized("User not authenticated");
                }

                _logger.LogInformation("POST /api/destinations/{DestinationId}/sharings called by user {UserId}", id, userId);

                // Validate comment
                if (string.IsNullOrWhiteSpace(dto.Comment))
                {
                    return this.BadRequest("Comment is required");
                }

                var sharing = await _destinationService.CreateDestinationSharingAsync(id, userId, dto);

                _logger.LogInformation("Destination sharing created successfully for destination {DestinationId}", id);

                return this.Success(sharing, "Destination sharing created successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed: {Message}", ex.Message);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating destination sharing for destination {DestinationId}", id);
                return this.InternalServerError("Failed to create destination sharing");
            }
        }

        /// <summary>
        /// Create a new destination (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> CreateDestination([FromForm] CreateDestinationRequest dto)
        {
            try
            {
                _logger.LogInformation("POST /api/destinations called - Creating new destination: {Name}", dto.Name);

                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return this.BadRequest("Destination name is required");
                }

                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    return this.BadRequest("Destination title is required");
                }

                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return this.BadRequest("Destination description is required");
                }

                if (dto.LocationId <= 0)
                {
                    return this.BadRequest("Valid location ID is required");
                }

                // Validate coordinates
                if (dto.Lat < -90 || dto.Lat > 90)
                {
                    return this.BadRequest("Latitude must be between -90 and 90");
                }

                if (dto.Lon < -180 || dto.Lon > 180)
                {
                    return this.BadRequest("Longitude must be between -180 and 180");
                }

                var destination = await _destinationService.CreateDestinationAsync(dto);

                _logger.LogInformation("Destination created successfully with id {DestinationId}", destination.Id);

                return this.Success(destination, "Destination created successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed: {Message}", ex.Message);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating destination");
                return this.InternalServerError("Failed to create destination");
            }
        }
    }
}
