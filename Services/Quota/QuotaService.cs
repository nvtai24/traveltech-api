using StackExchange.Redis;
using TravelTechApi.DTOs.Quota;

namespace TravelTechApi.Services.Quota
{
    public class QuotaService : IQuotaService
    {
        private readonly IConnectionMultiplexer _redis;

        public QuotaService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> CheckAndIncrementQuotaAsync(string featureName, string userId, int limit)
        {
            var db = _redis.GetDatabase();
            var key = GenerateQuotaKey(featureName, userId);

            // Get current usage count
            var currentUsage = await db.StringGetAsync(key);
            var usageCount = currentUsage.HasValue ? (int)currentUsage : 0;

            // Check if limit is reached
            if (usageCount >= limit)
            {
                return false;
            }

            // Increment the counter
            var newCount = await db.StringIncrementAsync(key);

            // Set expiration to end of day if this is the first increment
            if (newCount == 1)
            {
                var now = DateTime.Now;
                var endOfDay = now.Date.AddDays(1).AddSeconds(-1);
                var ttl = endOfDay - now;
                await db.KeyExpireAsync(key, ttl);
            }

            return true;
        }

        public async Task<int> GetCurrentUsageAsync(string featureName, string userId)
        {
            var db = _redis.GetDatabase();
            var key = GenerateQuotaKey(featureName, userId);
            var currentUsage = await db.StringGetAsync(key);
            return currentUsage.HasValue ? (int)currentUsage : 0;
        }

        public async Task ResetQuotaAsync(string featureName, string userId)
        {
            var db = _redis.GetDatabase();
            var key = GenerateQuotaKey(featureName, userId);
            await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Generates Redis key in format: quota:featureName:userId:ddMMyyyy
        /// </summary>
        private string GenerateQuotaKey(string featureName, string userId)
        {
            var dateKey = DateTime.Now.ToString("ddMMyyyy");
            return $"quota:{featureName}:{userId}:{dateKey}";
        }

        public async Task<QuotaResponse> CheckLimit(string featureName, string userId, int limit)
        {
            var db = _redis.GetDatabase();
            var key = GenerateQuotaKey(featureName, userId);
            var currentUsage = await db.StringGetAsync(key);
            var usageCount = currentUsage.HasValue ? (int)currentUsage : 0;
            return new QuotaResponse
            {
                Limit = limit,
                CurrentUsage = usageCount,
                HasRemainingQuota = usageCount < limit
            };
        }
    }
}
