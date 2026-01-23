namespace TravelTechApi.DTOs.Profile
{
    public class ProfileResponse
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public TravelTechApi.Entities.Gender? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}