using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Common;
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

        public async Task<PagedResult<GiftcodeResponse>> GetAllGiftcodesAsync(int page, int pageSize)
        {
            var query = _context.Giftcodes.AsQueryable();
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(g => g.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<GiftcodeResponse>>(items);

            return PagedResult<GiftcodeResponse>.Create(dtos, totalCount, page, pageSize);
        }

        public async Task<GiftcodeResponse?> GetGiftcodeByIdAsync(int id)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode == null) return null;
            return _mapper.Map<GiftcodeResponse>(giftcode);
        }

        public async Task<GiftcodeResponse?> GetGiftcodeByCodeAsync(string code)
        {
            var giftcode = await _context.Giftcodes.FirstOrDefaultAsync(g => g.Code == code);
            if (giftcode == null) return null;
            return _mapper.Map<GiftcodeResponse>(giftcode);
        }

        public async Task<GiftcodeResponse> CreateGiftcodeAsync(CreateGiftcodeRequest dto)
        {
            var giftcode = _mapper.Map<TravelTechApi.Entities.Giftcode>(dto);
            giftcode.CreatedAt = DateTime.UtcNow;

            _context.Giftcodes.Add(giftcode);
            await _context.SaveChangesAsync();

            return _mapper.Map<GiftcodeResponse>(giftcode);
        }

        public async Task<GiftcodeResponse?> UpdateGiftcodeAsync(int id, UpdateGiftcodeRequest dto)
        {
            var giftcode = await _context.Giftcodes.FindAsync(id);
            if (giftcode == null) return null;

            _mapper.Map(dto, giftcode);
            await _context.SaveChangesAsync();

            return _mapper.Map<GiftcodeResponse>(giftcode);
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
