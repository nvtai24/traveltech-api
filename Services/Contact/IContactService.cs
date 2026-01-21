using TravelTechApi.DTOs.Contact;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Contact;

public interface IContactService
{
    Task<bool> SendEmailAsync(ContactMessageRequest contactMessageRequest);
}