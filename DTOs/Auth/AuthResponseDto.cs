using TravelTechApi.DTOs.User;

namespace TravelTechApi.DTOs.Auth
{
    /// <summary>
    /// DTO for authentication response
    /// </summary>
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
    }
}
