using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs;

namespace TravelTechApi.Services.Travel
{
    /// <summary>
    /// Service implementation for Region operations
    /// </summary>
    public class RegionService : IRegionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionService> _logger;

        public RegionService(ApplicationDbContext context, IMapper mapper, ILogger<RegionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RegionDto>> GetAllRegionsAsync()
        {
            _logger.LogInformation("Getting all regions");
            var regions = await _context.Regions
                .OrderBy(r => r.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RegionDto>>(regions);
        }

        public async Task<RegionDto?> GetRegionByIdAsync(int id)
        {
            _logger.LogInformation("Getting region by id: {RegionId}", id);
            var region = await _context.Regions
                .FirstOrDefaultAsync(r => r.Id == id);

            return region == null ? null : _mapper.Map<RegionDto>(region);
        }
    }
}
