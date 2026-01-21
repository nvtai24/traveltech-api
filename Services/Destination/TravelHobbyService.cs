using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Destination;

namespace TravelTechApi.Services.Destination;

class TravelHobbyService : ITravelHobbyService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;


    public TravelHobbyService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TravelHobbyDto>> GetAllAsync()
    {
        var travelHobbies = await _context.TravelHobbies.ToListAsync();
        return _mapper.Map<List<TravelHobbyDto>>(travelHobbies);
    }

    public async Task<bool> UpdateAsync(TravelHobbyDto travelHobbyDto)
    {
        var travelHobby = await _context.TravelHobbies.FirstOrDefaultAsync(t => t.Id == travelHobbyDto.Id);
        if (travelHobby == null)
        {
            return false;
        }
        _mapper.Map(travelHobbyDto, travelHobby);
        await _context.SaveChangesAsync();
        return true;
    }
}