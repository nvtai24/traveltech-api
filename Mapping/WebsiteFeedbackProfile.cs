using AutoMapper;
using TravelTechApi.DTOs.WebsiteFeedback;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping;

public class WebsiteFeedbackProfile : Profile
{
    public WebsiteFeedbackProfile()
    {
        CreateMap<WebsiteFeedbackRequest, WebsiteFeedback>();
        CreateMap<WebsiteFeedback, WebsiteFeedbackResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email ?? string.Empty));
    }
}