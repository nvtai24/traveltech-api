namespace TravelTechApi.Entities
{
    /// <summary>
    /// Subscription plan entity representing service plans (Basic, Premium, etc.)
    /// </summary>
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        /// <summary>
        /// Plan name (e.g., Basic, Premium)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Plan description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Plan price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Daily usage limit for this plan
        /// </summary>
        public int DailyLimit { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Navigation property for user subscriptions
        /// </summary>
        public virtual ICollection<UserPlanSubscription> Users { get; set; } = new List<UserPlanSubscription>();
    }
}
