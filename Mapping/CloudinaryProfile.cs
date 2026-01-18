using AutoMapper;
using TravelTechApi.Entities;
using TravelTechApi.Services.Cloudinary;

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