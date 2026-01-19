namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for user information
    /// </summary>
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
