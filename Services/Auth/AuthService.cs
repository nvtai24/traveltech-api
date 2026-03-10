using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelTechApi.Common;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Common.Settings;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Auth;
using TravelTechApi.Entities;
using TravelTechApi.Services.Email;
using TravelTechApi.Services.UserPlanSubscription;
using Google.Apis.Auth;

namespace TravelTechApi.Services.Auth
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
        private readonly IUserPlanSubscriptionService _userPlanSubscriptionService;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            IMapper mapper,
            ILogger<AuthService> logger,
            IEmailService emailService,
            IUserPlanSubscriptionService userPlanSubscriptionService,
            IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _userPlanSubscriptionService = userPlanSubscriptionService;
            _googleAuthSettings = googleAuthSettings.Value;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerDto)
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

            // Assign default Basic subscription plan
            var basicPlan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Name == "Basic");

            if (basicPlan != null)
            {
                var subscription = new Entities.UserPlanSubscription
                {
                    UserId = user.Id,
                    SubscriptionPlanId = basicPlan.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(100_000) // free plan
                };

                await _context.UserPlanSubscriptions.AddAsync(subscription);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Assigned Basic subscription to new user: {UserId}", user.Id);
            }
            else
            {
                _logger.LogWarning("Basic subscription plan not found, user registered without subscription");
            }

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

            // Return user info without tokens - user must confirm email and login to get tokens
            return new RegisterResponse
            {
                User = _mapper.Map<UserResponse>(user),
                Message = "Registration successful. Please check your email to confirm your account before logging in."
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            // var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var user = await _context.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check if user is locked out
            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Login failed - user locked out: {Email}", loginDto.Email);
                throw new UnauthorizedException($"Your account is locked until {user.LockoutEnd}. Please contact support.");
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

        public async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            _logger.LogInformation("Google login attempt");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _googleAuthSettings.ClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID token");
                throw new BadRequestException("Invalid Google ID token");
            }

            var user = await _context.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                _logger.LogInformation("User not found, registering new user from Google login: {Email}", payload.Email);

                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed for {Email}. Errors: {Errors}",
                        payload.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new BadRequestException("Failed to create user from Google login");
                }

                await _userManager.AddToRoleAsync(user, AppRoles.User);

                // Assign basic plan
                var basicPlan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Name == "Basic");

                if (basicPlan != null)
                {
                    var subscription = new Entities.UserPlanSubscription
                    {
                        UserId = user.Id,
                        SubscriptionPlanId = basicPlan.Id,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(100_000) // free plan
                    };

                    await _context.UserPlanSubscriptions.AddAsync(subscription);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Update avatar if missing (Logic to be implemented if ExternalAvatarUrl is added)
                // for now, we rely on Cloudinary Avatar or manually updated profile

                var avatarUrl = payload.Picture;
                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    // user.AvatarUrl = avatarUrl;
                    // await _userManager.UpdateAsync(user);
                }

                if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
                {
                    _logger.LogWarning("Login failed - user locked out: {Email}", user.Email);
                    throw new UnauthorizedException($"Your account is locked until {user.LockoutEnd}. Please contact support.");
                }


            }

            _logger.LogInformation("Google login successful for user: {UserId}, Email: {Email}", user.Id, user.Email);
            return await GenerateAuthResponse(user);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            _logger.LogInformation("Token refresh attempt");

            var tx = await _context.Database.BeginTransactionAsync();

            // Find refresh token in database
            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenRequest.RefreshToken);

            if (storedRefreshToken == null)
            {
                _logger.LogWarning("Token refresh failed - refresh token not found");
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Validate refresh token
            // Maybe hacker use old refresh token to get new access token
            // Best practice: Revoke all refresh tokens of user immediately when this case occur
            // Refresh Token Rotation
            if (storedRefreshToken.IsUsed)
            {
                await RevokeTokenAsync(storedRefreshToken.UserId);
                await tx.CommitAsync();
                _logger.LogWarning("Token refresh failed - token already used. UserId: {UserId}", storedRefreshToken.UserId);
                throw new UnauthorizedException("Refresh token reuse detected. All sessions revoked.");
            }

            if (storedRefreshToken.IsRevoked)
            {
                _logger.LogWarning("Token refresh failed - token revoked. UserId: {UserId}", storedRefreshToken.UserId);
                throw new UnauthorizedException("Refresh token has been revoked");
            }

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Token refresh failed - token expired. UserId: {UserId}, ExpiresAt: {ExpiresAt}",
                    storedRefreshToken.UserId, storedRefreshToken.ExpiresAt);
                throw new UnauthorizedException("Refresh token has expired");
            }

            _logger.LogDebug("Validating refresh token for user: {UserId}", storedRefreshToken.UserId);

            // Mark old refresh token as used
            storedRefreshToken.IsUsed = true;
            storedRefreshToken.IsRevoked = true; // mark as revoked, to prevent reuse
            await _context.SaveChangesAsync();
            _logger.LogDebug("Marked refresh token as used for user: {UserId}", storedRefreshToken.UserId);

            // Get user to generate new tokens
            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            if (user == null)
            {
                _logger.LogError("User not found during token refresh: {UserId}", storedRefreshToken.UserId);
                throw new NotFoundException("User not found");
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var subscriptionPlan = await _userPlanSubscriptionService.GetCurrentPlanAsync(user.Id);
            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Get JTI from new access token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(newAccessToken);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;

            // Save new refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                JwtId = jti,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            _logger.LogDebug("Saved new refresh token to database. JTI: {Jti}, ExpiresAt: {ExpiresAt}",
                jti, refreshTokenEntity.ExpiresAt);

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", storedRefreshToken.UserId);

            // Return only tokens, no user info
            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
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

        private async Task<LoginResponse> GenerateAuthResponse(ApplicationUser user)
        {
            _logger.LogDebug("Generating auth response for user: {UserId}", user.Id);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, roles);
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

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = roles.ToList();

            var plan = await _userPlanSubscriptionService.GetCurrentPlanAsync(user.Id);
            userResponse.SubscriptionPlan = plan?.Name ?? string.Empty;

            // Handle IsFirstLogin logic
            if (user.IsFirstLogin)
            {
                user.IsFirstLogin = false;
                _context.Users.Update(user); // Important to update user status
                await _context.SaveChangesAsync();
            }

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = userResponse
            };
        }

        public async Task ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Forgot password request for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that user doesn't exist
                _logger.LogWarning("Forgot password failed - user not found: {Email}", email);
                return;
            }

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send email
            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Id, token);
                _logger.LogInformation("Password reset email sent to: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to: {Email}", email);
                throw new BadRequestException("Failed to send password reset email");
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            _logger.LogInformation("Reset password attempt for email: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Reset password failed - user not found: {Email}", request.Email);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for user: {Email}", request.Email);
            }
            else
            {
                _logger.LogWarning("Password reset failed for user: {Email}. Errors: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            _logger.LogInformation("Change password attempt for user: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Change password failed - user not found: {UserId}", userId);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password changed successful for user: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Change password failed for user: {UserId}. Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }
    }
}
