using TravelTechApi.DTOs;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for Destination operations
    /// </summary>
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword);
        Task<DestinationDetailsDto?> GetDestinationByIdAsync(int id);
        Task<IEnumerable<DestinationSharingDto>> GetDestinationsSharingsAsync(int destinationId);

    }
}
