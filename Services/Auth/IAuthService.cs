using Microsoft.AspNetCore.Identity;
using TravelTechApi.DTOs.Auth;

namespace TravelTechApi.Services.Auth
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
        Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request);
        Task ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }
}
