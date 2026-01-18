using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Interfaces
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
