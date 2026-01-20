using System.Security.Claims;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Auth
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
