using TravelTechApi.DTOs;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for Destination operations
    /// </summary>
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllDestinationsAsync();
        Task<IEnumerable<DestinationDto>> GetDestinationsByLocationIdAsync(int locationId);
        Task<DestinationDto?> GetDestinationByIdAsync(int id);
    }
}
