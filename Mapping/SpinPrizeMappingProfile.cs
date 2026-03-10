using AutoMapper;
using TravelTechApi.DTOs.SpinPrize;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class SpinPrizeMappingProfile : Profile
    {
        public SpinPrizeMappingProfile()
        {
            CreateMap<SpinPrize, SpinPrizeDto>();
            CreateMap<CreateSpinPrizeDto, SpinPrize>();
            CreateMap<UpdateSpinPrizeDto, SpinPrize>();
        }
    }
}
