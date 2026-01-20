using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination
{
    /// <summary>
    /// Service for Region operations
    /// </summary>
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> GetAllRegionsAsync();
    }
}
