using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs;
using Microsoft.Extensions.Logging;
using TravelTechApi.Services;

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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("POST /api/auth/register called for email: {Email}", registerDto.Email);
            var result = await _authService.RegisterAsync(registerDto);
            _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
            return this.Created(result, "User registered successfully");
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("POST /api/auth/login called for email: {Email}", loginDto.Email);
            var result = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("Login successful for email: {Email}", loginDto.Email);
            return this.Success(result, "Login successful");
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            _logger.LogInformation("POST /api/auth/refresh-token called");
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
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
            _logger.LogInformation("POST /api/auth/revoke-token called");
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
        /// Get current user information (requires authentication)
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            _logger.LogInformation("GET /api/auth/me called");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);

            var userDto = new UserDto
            {
                Id = userId ?? string.Empty,
                Email = email ?? string.Empty,
                FirstName = firstName,
                LastName = lastName
            };

            _logger.LogDebug("Retrieved current user info for: {UserId}", userId);

            return this.Success(userDto, "User retrieved successfully");
        }
    }
}
