using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination;

public interface ITravelHobbyService
{
    Task<List<TravelHobbyDto>> GetAllAsync();
    Task<bool> UpdateAsync(TravelHobbyDto travelHobbyDto);
}