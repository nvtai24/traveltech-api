using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs;

namespace TravelTechApi.Services.Travel
{
    /// <summary>
    /// Service implementation for Destination operations
    /// </summary>
    public class DestinationService : IDestinationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DestinationService> _logger;

        public DestinationService(ApplicationDbContext context, IMapper mapper, ILogger<DestinationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DestinationDto>> GetAllDestinationsAsync()
        {
            _logger.LogInformation("Getting all destinations");
            var destinations = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<IEnumerable<DestinationDto>> GetDestinationsByLocationIdAsync(int locationId)
        {
            _logger.LogInformation("Getting destinations by location id: {LocationId}", locationId);
            var destinations = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .Where(d => d.LocationId == locationId)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<DestinationDto?> GetDestinationByIdAsync(int id)
        {
            _logger.LogInformation("Getting destination by id: {DestinationId}", id);
            var destination = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .FirstOrDefaultAsync(d => d.Id == id);

            return destination == null ? null : _mapper.Map<DestinationDto>(destination);
        }
    }
}
