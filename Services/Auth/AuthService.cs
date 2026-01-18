using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelTechApi.Common;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Common.Settings;
using TravelTechApi.Data;
using TravelTechApi.DTOs;
using TravelTechApi.Entities;
using Microsoft.Extensions.Logging;
using TravelTechApi.Common.Constants;

namespace TravelTechApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            IMapper mapper,
            ILogger<AuthService> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Starting user registration for email: {Email}", registerDto.Email);

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - user already exists: {Email}", registerDto.Email);
                throw new ConflictException("User with this email already exists");
            }

            // Create new user using AutoMapper
            var user = _mapper.Map<ApplicationUser>(registerDto);
            _logger.LogDebug("Mapped RegisterDto to ApplicationUser for email: {Email}", registerDto.Email);

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}. Errors: {Errors}",
                    registerDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

                var errors = result.Errors.Select(e => new ErrorDetail
                {
                    Code = e.Code,
                    Message = e.Description
                }).ToList();

                throw new BadRequestException("Failed to create user", errors);
            }

            _logger.LogInformation("User created successfully: {UserId}, Email: {Email}", user.Id, user.Email);

            // Add default role to user
            await _userManager.AddToRoleAsync(user, AppRoles.User);

            // Generate email confirmation token
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send confirmation email
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email!, user.Id, emailToken);
                _logger.LogInformation("Confirmation email sent to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to: {Email}", user.Email);
                // Don't fail registration if email fails, user can resend later
            }



            // Generate tokens (user can still get tokens but login will check email confirmation)
            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed - email not confirmed: {Email}", loginDto.Email);
                throw new UnauthorizedException("Please confirm your email before logging in. Check your inbox for the confirmation link.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed - invalid password for user: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            _logger.LogInformation("Login successful for user: {UserId}, Email: {Email}", user.Id, user.Email);
            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            _logger.LogInformation("Token refresh attempt");

            // Validate access token
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            if (principal == null)
            {
                _logger.LogWarning("Token refresh failed - invalid access token");
                throw new UnauthorizedException("Invalid access token");
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Token refresh failed - invalid token claims");
                throw new UnauthorizedException("Invalid token claims");
            }

            _logger.LogDebug("Validating refresh token for user: {UserId}", userId);

            // Find refresh token in database
            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken && rt.UserId == userId);

            if (storedRefreshToken == null)
            {
                _logger.LogWarning("Token refresh failed - refresh token not found for user: {UserId}", userId);
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Validate refresh token
            if (storedRefreshToken.IsUsed)
            {
                _logger.LogWarning("Token refresh failed - token already used. UserId: {UserId}", userId);
                throw new UnauthorizedException("Refresh token has already been used");
            }

            if (storedRefreshToken.IsRevoked)
            {
                _logger.LogWarning("Token refresh failed - token revoked. UserId: {UserId}", userId);
                throw new UnauthorizedException("Refresh token has been revoked");
            }

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Token refresh failed - token expired. UserId: {UserId}, ExpiresAt: {ExpiresAt}",
                    userId, storedRefreshToken.ExpiresAt);
                throw new UnauthorizedException("Refresh token has expired");
            }

            // Get JTI from access token
            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (storedRefreshToken.JwtId != jti)
            {
                _logger.LogWarning("Token refresh failed - JTI mismatch. UserId: {UserId}", userId);
                throw new UnauthorizedException("Token mismatch");
            }

            // Mark old refresh token as used
            storedRefreshToken.IsUsed = true;
            await _context.SaveChangesAsync();
            _logger.LogDebug("Marked refresh token as used for user: {UserId}", userId);

            // Get user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User not found during token refresh: {UserId}", userId);
                throw new NotFoundException("User not found");
            }

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", userId);

            // Generate new tokens
            return await GenerateAuthResponse(user);
        }

        public async Task RevokeTokenAsync(string userId)
        {
            _logger.LogInformation("Revoking all tokens for user: {UserId}", userId);

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Revoked {Count} tokens for user: {UserId}", refreshTokens.Count, userId);
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            _logger.LogInformation("Email confirmation attempt for user: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed - user not found: {UserId}", userId);
                throw new NotFoundException("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for user: {UserId}", userId);
                return true;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed for user: {UserId}. Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));

                var errors = result.Errors.Select(e => new ErrorDetail
                {
                    Code = e.Code,
                    Message = e.Description
                }).ToList();

                throw new BadRequestException("Failed to confirm email", errors);
            }

            _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);
            return true;
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            _logger.LogInformation("Resending confirmation email to: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Resend confirmation failed - user not found: {Email}", email);
                // Don't reveal that user doesn't exist for security
                return;
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for: {Email}", email);
                // Don't reveal that email is already confirmed
                return;
            }

            // Generate new confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send confirmation email
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email!, user.Id, token);
                _logger.LogInformation("Confirmation email resent to: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend confirmation email to: {Email}", email);
                throw new BadRequestException("Failed to send confirmation email. Please try again later.");
            }
        }

        private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
        {
            _logger.LogDebug("Generating auth response for user: {UserId}", user.Id);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Get JTI from access token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = jti,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Saved refresh token to database. JTI: {Jti}, ExpiresAt: {ExpiresAt}",
                jti, refreshTokenEntity.ExpiresAt);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}
