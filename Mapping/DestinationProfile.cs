using AutoMapper;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    /// <summary>
    /// AutoMapper profile for Region, Location, and Destination mappings
    /// </summary>
    public class DestinationProfile : Profile
    {
        public DestinationProfile()
        {
            // Region mappings
            CreateMap<Region, RegionDto>();

            // Location mappings
            CreateMap<Location, LocationResponse>()
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region.Name));

            // Destination mappings
            CreateMap<Destination, DestinationResponse>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).FirstOrDefault()));

            CreateMap<Destination, DestinationAdminResponse>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).FirstOrDefault()));

            CreateMap<Destination, DestinationDetailsAdminResponse>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.FAQs, opt => opt.MapFrom(src => src.FAQs));

            CreateMap<FAQ, FaqDto>();

            CreateMap<Destination, DestinationDetailsResponse>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.FAQs, opt => opt.MapFrom(src => src.FAQs));

            CreateMap<DestinationSharing, DestinationSharingResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()));

            // Create mappings
            CreateMap<CreateDestinationRequest, Destination>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.FAQs, opt => opt.Ignore())
                .ForMember(dest => dest.Sharings, opt => opt.Ignore());

            CreateMap<FaqDto, FAQ>();
            CreateMap<UpdateFAQRequest, FAQ>();



        }
    }
}
