using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Giftcode;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Giftcode
{
    public class GiftcodeService : IGiftcodeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GiftcodeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GiftcodeDto>> GetAllGiftcodesAsync()
        {
            var giftcodes = await _context.Giftcodes.ToListAsync();
            return _mapper.Map<List<GiftcodeDto>>(giftcodes);
        }

        public async Task<GiftcodeDto?> GetGiftcodeByIdAsync(int id)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode == null) return null;
            return _mapper.Map<GiftcodeDto>(giftcode);
        }

        public async Task<GiftcodeDto?> GetGiftcodeByCodeAsync(string code)
        {
            var giftcode = await _context.Giftcodes.FirstOrDefaultAsync(g => g.Code == code);
            if (giftcode == null) return null;
            return _mapper.Map<GiftcodeDto>(giftcode);
        }

        public async Task<GiftcodeDto> CreateGiftcodeAsync(CreateGiftcodeDto dto)
        {
            var giftcode = _mapper.Map<TravelTechApi.Entities.Giftcode>(dto);
            giftcode.CreatedAt = DateTime.UtcNow;

            _context.Giftcodes.Add(giftcode);
            await _context.SaveChangesAsync();

            return _mapper.Map<GiftcodeDto>(giftcode);
        }

        public async Task<GiftcodeDto?> UpdateGiftcodeAsync(int id, UpdateGiftcodeDto dto)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode == null) return null;

            _mapper.Map(dto, giftcode);
            await _context.SaveChangesAsync();

            return _mapper.Map<GiftcodeDto>(giftcode);
        }

        public async Task<bool> DeleteGiftcodeAsync(int id)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode == null) return false;

            _context.Giftcodes.Remove(giftcode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateGiftcodeAsync(string code)
        {
            var giftcode = await _context.Giftcodes.FirstOrDefaultAsync(g => g.Code == code);
            if (giftcode == null) return false;

            if (!giftcode.IsActive) return false;

            var now = DateTime.UtcNow;
            if (now < giftcode.ValidFrom || now > giftcode.ValidTo) return false;

            if (giftcode.UsageCount >= giftcode.UsageLimit) return false;

            return true;
        }

        public async Task IncrementUsageAsync(int id)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode != null)
            {
                giftcode.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}
