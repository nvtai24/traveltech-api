using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for user login
    /// </summary>
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserResponse User { get; set; } = new();
        public bool IsFirstLogin { get; set; }
    }
}
