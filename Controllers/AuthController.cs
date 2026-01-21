using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Auth;
using TravelTechApi.Services.Auth;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Authentication controller for user registration, login, and token management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            _logger.LogInformation("User registered successfully: {Email}", registerRequest.Email);
            return this.Created(result, result.Message);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);
            _logger.LogInformation("Login successful for email: {Email}", loginRequest.Email);
            return this.Success(result, "Login successful");
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
            _logger.LogInformation("Token refreshed successfully");
            return this.Success(result, "Token refreshed successfully");
        }

        /// <summary>
        /// Revoke all refresh tokens for the current user (logout)
        /// </summary>
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Revoke token failed - user ID not found in claims");
                return this.Unauthorized("User not found");
            }

            await _authService.RevokeTokenAsync(userId);
            _logger.LogInformation("Tokens revoked successfully for user: {UserId}", userId);
            return this.Success("All tokens revoked successfully");
        }

        /// <summary>
        /// Confirm user email with token
        /// </summary>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
        {
            var result = await _authService.ConfirmEmailAsync(confirmEmailRequest.UserId, confirmEmailRequest.Token);
            _logger.LogInformation("Email confirmed successfully for userId: {UserId}", confirmEmailRequest.UserId);
            return this.Success(result, "Email confirmed successfully. You can now login.");
        }

        /// <summary>
        /// Resend email confirmation
        /// </summary>
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationRequest resendConfirmationRequest)
        {
            await _authService.ResendConfirmationEmailAsync(resendConfirmationRequest.Email);
            _logger.LogInformation("Confirmation email resent to: {Email}", resendConfirmationRequest.Email);
            return this.Success("If the email exists and is not confirmed, a confirmation email has been sent.");
        }

        /// <summary>
        /// Get current user information (requires authentication)
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            // var gender = User.FindFirstValue("Gender");
            // var dob = User.FindFirstValue("Dob");

            var userDto = new UserResponse
            {
                Id = userId ?? string.Empty,
                Email = email ?? string.Empty,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                PhoneNumber = phoneNumber ?? string.Empty
            };

            _logger.LogDebug("Retrieved current user info for: {UserId}", userId);

            return this.Success(userDto, "User retrieved successfully");
        }
    }
}
