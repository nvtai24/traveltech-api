using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs;

namespace TravelTechApi.Services.Destination
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

        public async Task<IEnumerable<DestinationDto>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword)
        {
            _logger.LogInformation("Getting destinations by region id: {RegionId}, location id: {LocationId}, keyword: {Keyword}", regionId, locationId, keyword);
            var destinations = await _context.Destinations
                .Include(d => d.Location)
                .ThenInclude(l => l.Region)
                .Include(d => d.Images)
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (regionId.HasValue)
            {
                destinations = destinations.Where(d => d.Location.RegionId == regionId.Value).ToList();
            }

            if (locationId.HasValue)
            {
                destinations = destinations.Where(d => d.LocationId == locationId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                destinations = destinations.Where(d => d.Name.ToLower().Contains(keyword)
                 || d.Title.ToLower().Contains(keyword)
                 || d.Description.ToLower().Contains(keyword)
                 || d.History.ToLower().Contains(keyword)
                 || d.Tags.Any(tag => tag.ToLower().Contains(keyword))).ToList();
            }

            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<DestinationDetailsDto?> GetDestinationByIdAsync(int id)
        {
            _logger.LogInformation("Getting destination by id: {DestinationId}", id);
            var destination = await _context.Destinations
                .Include(d => d.Location)
                .Include(d => d.Images)
                .Include(d => d.FAQs)
                .FirstOrDefaultAsync(d => d.Id == id);

            return destination == null ? null : _mapper.Map<DestinationDetailsDto>(destination);
        }
        public async Task<IEnumerable<DestinationSharingDto>> GetDestinationsSharingsAsync(int destinationId)
        {
            _logger.LogInformation("Getting destinations sharings by destination id: {DestinationId}", destinationId);
            var destinationSharings = await _context.DestinationSharings
                .Include(ds => ds.User)
                .Where(ds => ds.DestinationId == destinationId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DestinationSharingDto>>(destinationSharings);
        }

    }
}
