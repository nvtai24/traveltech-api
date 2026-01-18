using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Services.Interfaces;

namespace TravelTechApi.Services.Destination
{
    /// <summary>
    /// Service implementation for Location operations
    /// </summary>
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(ApplicationDbContext context, IMapper mapper, ILogger<LocationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            _logger.LogInformation("Getting all locations");
            var locations = await _context.Locations
                .Include(l => l.Region)
                .OrderBy(l => l.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsByRegionIdAsync(int regionId)
        {
            _logger.LogInformation("Getting locations by region id: {RegionId}", regionId);
            var locations = await _context.Locations
                .Include(l => l.Region)
                .Where(l => l.RegionId == regionId)
                .OrderBy(l => l.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            _logger.LogInformation("Getting location by id: {LocationId}", id);
            var location = await _context.Locations
                .Include(l => l.Region)
                .FirstOrDefaultAsync(l => l.Id == id);

            return location == null ? null : _mapper.Map<LocationDto>(location);
        }
    }
}
