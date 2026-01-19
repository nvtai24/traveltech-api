using AutoMapper;
using TravelTechApi.DTOs.Auth;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    /// <summary>
    /// AutoMapper profile for ApplicationUser to UserDto mapping
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();

            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
