using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.Giftcode;

namespace TravelTechApi.Services.Giftcode
{
    public interface IGiftcodeService
    {
        Task<PagedResult<GiftcodeDto>> GetAllGiftcodesAsync(int page, int pageSize);
        Task<GiftcodeDto?> GetGiftcodeByIdAsync(int id);
        Task<GiftcodeDto?> GetGiftcodeByCodeAsync(string code);
        Task<GiftcodeDto> CreateGiftcodeAsync(CreateGiftcodeDto dto);
        Task<GiftcodeDto?> UpdateGiftcodeAsync(int id, UpdateGiftcodeDto dto);
        Task<bool> DeleteGiftcodeAsync(int id);
        Task<bool> ValidateGiftcodeAsync(string code);
        Task IncrementUsageAsync(int id);
    }
}
