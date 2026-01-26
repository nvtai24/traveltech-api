using AutoMapper;
using TravelTechApi.DTOs.Giftcode;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class GiftcodeProfile : Profile
    {
        public GiftcodeProfile()
        {
            CreateMap<Giftcode, GiftcodeResponse>();
            CreateMap<CreateGiftcodeRequest, Giftcode>();
            CreateMap<UpdateGiftcodeRequest, Giftcode>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
