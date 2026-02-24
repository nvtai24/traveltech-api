using AutoMapper;
using TravelTechApi.DTOs.BlogComment;

namespace TravelTechApi.Mapping
{
    public class BlogCommentProfile : Profile
    {
        public BlogCommentProfile()
        {
            CreateMap<Entities.BlogComment, BlogCommentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "Unknown"))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.User != null && src.User.Avatar != null ? src.User.Avatar.Url : null));
        }
    }
}
