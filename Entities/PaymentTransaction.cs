namespace TravelTechApi.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
        public int SubscriptionPlanId { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public decimal Amount { get; set; }
        // public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }

        // SePay / Bank fields
        public long TransactionId { get; set; } // Bank transaction ID
        public string? Gateway { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankCode { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
