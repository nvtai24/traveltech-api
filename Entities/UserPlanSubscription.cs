namespace TravelTechApi.Entities
{
    /// <summary>
    /// Join entity for many-to-many relationship between ApplicationUser and SubscriptionPlan
    /// Represents subscription history
    /// </summary>
    public class UserPlanSubscription
    {
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to ApplicationUser
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Foreign key to SubscriptionPlan
        /// </summary>
        public int SubscriptionPlanId { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        /// <summary>
        /// Subscription start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Subscription end date
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}