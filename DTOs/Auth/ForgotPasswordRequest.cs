using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
