using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.UserPlanSubscription
{
    public class UserPlanSubscriptionService : IUserPlanSubscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserPlanSubscriptionService> _logger;

        public UserPlanSubscriptionService(ApplicationDbContext context, ILogger<UserPlanSubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SubscriptionPlan?> GetCurrentPlanAsync(string userId)
        {
            var subscription = await _context.UserPlanSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.SubscriptionPlan.Order)
                .FirstOrDefaultAsync();


            if (subscription == null)
            {
                return null;
            }

            return subscription.SubscriptionPlan;
        }

        public async Task<bool> IsSubscriptionExpiredAsync(string userId)
        {
            var subscription = await _context.UserPlanSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (subscription == null)
            {
                return true;
            }

            return subscription.EndDate < DateTime.UtcNow;
        }

    }
}