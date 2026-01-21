using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination;

public class PriceSettingService : IPriceSettingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PriceSettingService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PriceSettingDto>> GetPriceSettingsAsync()
    {
        var priceSettings = await _context.PriceSettings.ToListAsync();
        return _mapper.Map<List<PriceSettingDto>>(priceSettings);
    }

    public async Task<bool> UpdatePriceSettingsAsync(PriceSettingDto priceSettingsDto)
    {
        var priceSettings = await _context.PriceSettings.FirstOrDefaultAsync(p => p.Id == priceSettingsDto.Id);
        if (priceSettings == null)
        {
            return false;
        }
        _mapper.Map(priceSettingsDto, priceSettings);
        await _context.SaveChangesAsync();
        return true;
    }
}