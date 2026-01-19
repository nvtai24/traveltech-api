using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for email confirmation request
    /// </summary>
    public class ConfirmEmailRequest
    {
        /// <summary>
        /// User ID
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Email confirmation token
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for resending email confirmation
    /// </summary>
    public class ResendConfirmationRequest
    {
        /// <summary>
        /// User email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
