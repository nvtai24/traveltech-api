using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Contact;

public class ContactMessageRequest
{
    public string FullName { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int ContactTopicId { get; set; }
}
