using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination
{
    /// <summary>
    /// Service for Destination operations
    /// </summary>
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationResponse>> GetAllDestinationsAsync(int? regionId, int? locationId, string? keyword);
        Task<DestinationDetailsResponse?> GetDestinationByIdAsync(int id);
        Task<IEnumerable<DestinationSharingResponse>> GetDestinationsSharingsAsync(int destinationId);
        Task<DestinationSharingResponse> CreateDestinationSharingAsync(int destinationId, string userId, CreateDestinationSharingRequest dto);
        Task<DestinationDetailsResponse> CreateDestinationAsync(CreateDestinationRequest dto);

    }
}
