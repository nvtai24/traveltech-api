using AutoMapper;
using TravelTechApi.DTOs;
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
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region.Name));

            // Destination mappings
            CreateMap<Destination, DestinationDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).First()));

            CreateMap<FAQ, FaqDto>();

            CreateMap<Destination, DestinationDetailsDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.FAQs, opt => opt.MapFrom(src => src.FAQs));

            CreateMap<DestinationSharing, DestinationSharingDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()));
            ;

        }
    }
}
