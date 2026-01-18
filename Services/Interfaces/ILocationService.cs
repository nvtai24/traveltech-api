using TravelTechApi.DTOs;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for Location operations
    /// </summary>
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<IEnumerable<LocationDto>> GetLocationsByRegionIdAsync(int regionId);
        Task<LocationDto?> GetLocationByIdAsync(int id);
    }
}
