using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.SpinPrize;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.SpinPrize
{
    public class SpinPrizeService : ISpinPrizeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SpinPrizeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
    }
}
