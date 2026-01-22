using TravelTechApi.Entities;

namespace TravelTechApi.DTOs.Payment
{
    public class CreatePaymentRequest
    {
        public int SubscriptionPlanId { get; set; }
        // public PaymentMethod PaymentMethod { get; set; }
    }

    public class PaymentOrderResponse
    {
        public Guid PaymentId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string QRCodeUrl { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SepayWebhookRequest
    {
        public long Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }

    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string SubscriptionPlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        // public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
