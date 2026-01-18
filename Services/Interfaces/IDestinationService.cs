using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Interfaces
{
    /// <summary>
    /// Service for Destination operations
    /// </summary>
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword);
        Task<DestinationDetailsDto?> GetDestinationByIdAsync(int id);
        Task<IEnumerable<DestinationSharingDto>> GetDestinationsSharingsAsync(int destinationId);
        Task<DestinationSharingDto> CreateDestinationSharingAsync(int destinationId, string userId, CreateDestinationSharingDto dto);
        Task<DestinationDetailsDto> CreateDestinationAsync(CreateDestinationDto dto);

    }
}
