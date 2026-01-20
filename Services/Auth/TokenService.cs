using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TravelTechApi.Common.Settings;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            _logger.LogDebug("Generating access token for user: {UserId}", user.Id);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                // new Claim("Gender", user.Gender ?? string.Empty),
                // new Claim("Dob", user.Dob?.ToString("dd-MM-yyyy") ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("Access token generated for user: {UserId}, Expires: {ExpiresAt}",
                user.Id, token.ValidTo);
            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            _logger.LogDebug("Generating refresh token");
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            _logger.LogDebug("Refresh token generated successfully");
            return refreshToken;
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            _logger.LogDebug("Validating expired token");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Don't validate lifetime for expired tokens
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Invalid token algorithm");
                    return null;
                }

                _logger.LogDebug("Token validated successfully");
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return null;
            }
        }
    }
}
