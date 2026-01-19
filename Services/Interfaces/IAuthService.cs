using TravelTechApi.DTOs.Auth;

namespace TravelTechApi.Services.Interfaces
{
    /// <summary>
    /// Service for authentication operations
    /// </summary>
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerDto);
        Task<LoginResponse> LoginAsync(LoginRequest loginDto);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task RevokeTokenAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task ResendConfirmationEmailAsync(string email);
    }
}
