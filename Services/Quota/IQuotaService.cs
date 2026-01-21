using TravelTechApi.DTOs.Quota;

namespace TravelTechApi.Services.Quota
{
    /// <summary>
    /// Service for managing daily feature usage quotas using Redis cache
    /// </summary>
    public interface IQuotaService
    {
        /// <summary>
        /// Checks if the user has quota remaining for the feature and increments the counter if available
        /// </summary>
        /// <param name="featureName">Name of the feature to check quota for</param>
        /// <param name="userId">User ID to check quota for</param>
        /// <param name="limit">Maximum allowed usage count per day</param>
        /// <returns>True if quota is available and incremented, false if limit is reached</returns>
        Task<bool> CheckAndIncrementQuotaAsync(string featureName, string userId, int limit);

        /// <summary>
        /// Gets the current usage count for a feature
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="userId">User ID</param>
        /// <returns>Current usage count</returns>
        Task<int> GetCurrentUsageAsync(string featureName, string userId);

        /// <summary>
        /// Resets the quota for a specific feature and user (useful for testing)
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="userId">User ID</param>
        Task ResetQuotaAsync(string featureName, string userId);

        Task<QuotaResponse> CheckLimit(string featureName, string userId, int limit);
    }
}
