using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.Giftcode;

namespace TravelTechApi.Services.Giftcode
{
    public interface IGiftcodeService
    {
        Task<PagedResult<GiftcodeResponse>> GetAllGiftcodesAsync(int page, int pageSize);
        Task<GiftcodeResponse?> GetGiftcodeByIdAsync(int id);
        Task<GiftcodeResponse?> GetGiftcodeByCodeAsync(string code);
        Task<GiftcodeResponse> CreateGiftcodeAsync(CreateGiftcodeRequest dto);
        Task<GiftcodeResponse?> UpdateGiftcodeAsync(int id, UpdateGiftcodeRequest dto);
        Task<bool> DeleteGiftcodeAsync(int id);
        Task<bool> ValidateGiftcodeAsync(string code);
        Task IncrementUsageAsync(int id);
    }
}
