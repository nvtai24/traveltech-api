using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for token refresh
    /// </summary>
    public class RefreshTokenDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
