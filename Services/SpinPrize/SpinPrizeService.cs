using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TravelTechApi.Data;
using TravelTechApi.DTOs.SpinPrize;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.SpinPrize
{
    public class SpinPrizeService : ISpinPrizeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _redis;
        private const string RedisConfigKey = "SpinPrize:Config";

        public SpinPrizeService(ApplicationDbContext context, IMapper mapper, IConnectionMultiplexer redis)
        {
            _context = context;
            _mapper = mapper;
            _redis = redis;
        }

        public async Task<IEnumerable<SpinPrizeDto>> GetAllAdminAsync()
        {
            var prizes = await _context.SpinPrizes.ToListAsync();
            return _mapper.Map<IEnumerable<SpinPrizeDto>>(prizes);
        }

        public async Task<IEnumerable<SpinPrizeDto>> GetAllPublicAsync()
        {
            var prizes = await _context.SpinPrizes.Where(p => p.IsActive).ToListAsync();
            return _mapper.Map<IEnumerable<SpinPrizeDto>>(prizes);
        }

        public async Task<SpinPrizeDto> GetByIdAsync(Guid id)
        {
            var prize = await _context.SpinPrizes.FindAsync(id);
            if (prize == null)
            {
                return null;
            }
            return _mapper.Map<SpinPrizeDto>(prize);
        }

        public async Task<bool> CreateAsync(CreateSpinPrizeDto createDto)
        {
            var prize = _mapper.Map<Entities.SpinPrize>(createDto);

            _context.SpinPrizes.Add(prize);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSpinPrizeDto updateDto)
        {
            var prize = await _context.SpinPrizes.FindAsync(id);
            if (prize == null)
            {
                return false;
            }

            _mapper.Map(updateDto, prize);
            prize.UpdatedAt = DateTime.UtcNow;

            _context.SpinPrizes.Update(prize);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var prize = await _context.SpinPrizes.FindAsync(id);
            if (prize == null)
            {
                return false;
            }

            _context.SpinPrizes.Remove(prize);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<SpinPrizeConfigDto> GetConfigAsync()
        {
            var db = _redis.GetDatabase();
            var json = await db.StringGetAsync(RedisConfigKey);

            if (json.IsNullOrEmpty)
            {
                return new SpinPrizeConfigDto(); // Default properties configured in Dto
            }

            return JsonSerializer.Deserialize<SpinPrizeConfigDto>(json);
        }

        public async Task<bool> SaveConfigAsync(SpinPrizeConfigDto configDto)
        {
            configDto.UpdatedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(configDto);

            var db = _redis.GetDatabase();
            return await db.StringSetAsync(RedisConfigKey, json);
        }

        public async Task<bool> ClearConfigAsync()
        {
            var db = _redis.GetDatabase();
            return await db.KeyDeleteAsync(RedisConfigKey);
        }
    }
}
