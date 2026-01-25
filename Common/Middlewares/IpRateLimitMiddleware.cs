using System.Net;
using StackExchange.Redis;
using TravelTechApi.Common.Settings;
using TravelTechApi.Services.Auth;

namespace TravelTechApi.Common.Middlewares
{
    public class IpRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<IpRateLimitMiddleware> _logger;

        public IpRateLimitMiddleware(RequestDelegate next, IConnectionMultiplexer redis, ILogger<IpRateLimitMiddleware> logger)
        {
            _next = next;
            _redis = redis;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply to specific path
            if (context.Request.Path.StartsWithSegments("/api/plan/generate"))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var key = $"ratelimit:plan:generate:{ipAddress}";
                var limit = 5; // 5 requests per minute
                var period = TimeSpan.FromMinutes(1);

                var db = _redis.GetDatabase();

                var currentCount = await db.StringIncrementAsync(key);

                if (currentCount == 1)
                {
                    await db.KeyExpireAsync(key, period);
                }

                if (currentCount > limit)
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {IP}", ipAddress);
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsJsonAsync(new { message = "Rate limit exceeded. Please try again later." });
                    return;
                }
            }

            await _next(context);
        }
    }
}
