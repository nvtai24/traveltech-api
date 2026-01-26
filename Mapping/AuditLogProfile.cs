using AutoMapper;
using TravelTechApi.DTOs.Audit;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            CreateMap<AuditLog, AuditLogResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "System/Anonymous"));
        }
    }
}
