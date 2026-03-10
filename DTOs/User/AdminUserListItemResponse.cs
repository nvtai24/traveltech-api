using TravelTechApi.Entities;

namespace TravelTechApi.DTOs.User
{
    /// <summary>
    /// User list item response for admin
    /// </summary>
    public class AdminUserListItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string AvatarUrl { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string SubscriptionPlan { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
