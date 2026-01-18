using TravelTechApi.DTOs;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for Region operations
    /// </summary>
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> GetAllRegionsAsync();
        Task<RegionDto?> GetRegionByIdAsync(int id);
    }
}
