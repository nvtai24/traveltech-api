using AutoMapper;
using TravelTechApi.DTOs.Contact;
using TravelTechApi.Entities;

namespace TravelTechApi.Mapping;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<ContactMessageRequest, ContactMessage>();

        CreateMap<ContactTopic, ContactTopicDto>();
    }
}