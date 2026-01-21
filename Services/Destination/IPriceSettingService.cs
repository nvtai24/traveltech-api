using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination;

public interface IPriceSettingService
{
    Task<List<PriceSettingDto>> GetPriceSettingsAsync();
    Task<bool> UpdatePriceSettingsAsync(PriceSettingDto priceSettingsDto);
}