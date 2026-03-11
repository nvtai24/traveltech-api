using TravelTechApi.Entities;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for user information
    /// </summary>
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? Dob { get; set; }
    }
}
