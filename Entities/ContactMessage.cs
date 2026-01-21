using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities;

public class ContactMessage
{
    [Key]
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int ContactTopicId { get; set; }
    public ContactTopic ContactTopic { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}