using AutoMapper;
using TravelTechApi.DTOs.Blog;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<Blog, BlogPublicResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? $"{src.Author.FirstName} {src.Author.LastName}" : "Unknown"))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.Author != null && src.Author.Avatar != null ? src.Author.Avatar.Url : null));

            CreateMap<Blog, BlogAdminResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? $"{src.Author.FirstName} {src.Author.LastName}" : "Unknown"))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.Author != null && src.Author.Avatar != null ? src.Author.Avatar.Url : null))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedBy != null ? $"{src.UpdatedBy.FirstName} {src.UpdatedBy.LastName}" : null))
                .ForMember(dest => dest.UpdatedByAvatar, opt => opt.MapFrom(src => src.UpdatedBy != null && src.UpdatedBy.Avatar != null ? src.UpdatedBy.Avatar.Url : null));

            CreateMap<UpdateBlogRequest, Blog>();
        }
    }
}
