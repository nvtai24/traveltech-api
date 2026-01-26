using System.Security.Claims;

namespace TravelTechApi.Services.Auth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        ClaimsPrincipal? User { get; }
    }
}
