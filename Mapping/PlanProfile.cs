using AutoMapper;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.DTOs.Plan;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class PlanProfile : Profile
    {
        public PlanProfile()
        {
            // Plan mappings
            CreateMap<Entities.Plan, PlanResponse>()
                .ForMember(dest => dest.PriceSetting, opt => opt.MapFrom(src => src.PriceSetting.Name))
                .ForMember(dest => dest.Hobbies, opt => opt.MapFrom(src => src.Hobbies.Select(h => h.Name).ToList()))
                .ForMember(dest => dest.Accommodations, opt => opt.MapFrom(src => src.AccommodationRecommendations))
                .ForMember(dest => dest.Transportations, opt => opt.MapFrom(src => src.TransportationRecommendations));

            // DailyItinerary mappings
            CreateMap<DailyItinerary, DailyItineraryResponse>();

            // Activity mappings
            CreateMap<Activity, ActivityResponse>();

            // FoodRecommendation mappings
            CreateMap<FoodRecommendation, FoodRecommendationResponse>();

            // AccommodationRecommendation mappings
            CreateMap<AccommodationRecommendation, AccommodationRecommendationResponse>();

            // TransportationRecommendation mappings
            CreateMap<TransportationRecommendation, TransportationRecommendationResponse>();

            CreateMap<PriceSettingDto, PriceSetting>();
            CreateMap<PriceSetting, PriceSettingDto>();

            CreateMap<TravelHobbyDto, TravelHobby>();
            CreateMap<TravelHobby, TravelHobbyDto>();
        }
    }
}
