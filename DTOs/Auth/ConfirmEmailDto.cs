using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for email confirmation request
    /// </summary>
    public class ConfirmEmailDto
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
}
