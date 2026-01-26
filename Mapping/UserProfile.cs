using AutoMapper;
using TravelTechApi.DTOs.Auth;
using TravelTechApi.DTOs.User;
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
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar != null ? src.Avatar.SecureUrl : string.Empty));

            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Admin User Management mappings
            CreateMap<ApplicationUser, AdminUserResponse>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar != null ? src.Avatar.SecureUrl : string.Empty))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd > DateTimeOffset.UtcNow))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEnd.HasValue ? src.LockoutEnd.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.Ignore()); // Set manually

            CreateMap<ApplicationUser, AdminUserListItemResponse>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar != null ? src.Avatar.SecureUrl : string.Empty))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd > DateTimeOffset.UtcNow))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.Ignore()); // Set manually

            // Update mapping: only map non-null values
            CreateMap<UpdateUserRequest, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
