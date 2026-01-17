using TravelTechApi.DTOs;

namespace TravelTechApi.Services
{
    /// <summary>
    /// Service for authentication operations
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task RevokeTokenAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task ResendConfirmationEmailAsync(string email);
    }
}
