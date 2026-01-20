using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Data;
using TravelTechApi.Entities;
using TravelTechApi.Services.Quota;

namespace TravelTechApi.Services.UserPlanSubscription
{
    public class UserPlanSubscriptionService : IUserPlanSubscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQuotaService _quotaService;
        private readonly ILogger<UserPlanSubscriptionService> _logger;


        public UserPlanSubscriptionService(ApplicationDbContext context, IQuotaService quotaService, ILogger<UserPlanSubscriptionService> logger)
        {
            _context = context;
            _quotaService = quotaService;
            _logger = logger;
        }

        public async Task<SubscriptionPlan> GetCurrentPlanAsync(string userId)
        {
            var subscription = await _context.UserPlanSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.SubscriptionPlan.Order)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                throw new NotFoundException("User subscription not found");
            }

            return subscription.SubscriptionPlan;
        }

        public async Task<bool> IsPlanLimitedAsync(string userId)
        {
            var plan = await GetCurrentPlanAsync(userId);
            var limit = plan.DailyLimit;

            return !await _quotaService.CheckAndIncrementQuotaAsync("ai_plan_generation", userId, limit);
        }

    }
}