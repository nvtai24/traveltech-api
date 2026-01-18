using AutoMapper;
using TravelTechApi.DTOs.Cloudinary;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping
{
    public class CloudinaryProfile : Profile
    {
        public CloudinaryProfile()
        {
            CreateMap<CloudinaryUploadResult, CloudinaryFileInfo>();
        }
    }
}