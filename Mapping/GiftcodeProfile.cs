using AutoMapper;
using TravelTechApi.DTOs.Giftcode;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class GiftcodeProfile : Profile
    {
        public GiftcodeProfile()
        {
            CreateMap<Giftcode, GiftcodeDto>();
            CreateMap<CreateGiftcodeDto, Giftcode>();
            CreateMap<UpdateGiftcodeDto, Giftcode>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
