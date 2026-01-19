using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Interfaces
{
    /// <summary>
    /// Service for Location operations
    /// </summary>
    public interface ILocationService
    {
        Task<IEnumerable<LocationResponse>> GetAllLocationsAsync();
        Task<IEnumerable<LocationResponse>> GetLocationsByRegionIdAsync(int regionId);
        Task<LocationResponse?> GetLocationByIdAsync(int id);
    }
}
