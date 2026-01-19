using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Auth
{
    public class RefreshTokenRequest
    {

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }


    /// <summary>
    /// DTO for token refresh
    /// </summary>
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

}
