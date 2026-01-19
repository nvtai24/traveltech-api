using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for user registration
    /// </summary>
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }

    public class RegisterResponse
    {
        public UserResponse User { get; set; } = new();
        public string Message { get; set; } = "Registration successful. Please check your email to confirm your account.";
    }
}
