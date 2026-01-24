using TravelTechApi.Common;
using TravelTechApi.DTOs.Payment;

namespace TravelTechApi.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentOrderResponse> CreatePaymentOrderAsync(string userId, CreatePaymentRequest dto);
        Task<PaymentTransactionResponse> GetPaymentByIdAsync(Guid paymentId, string userId);
        Task<List<PaymentTransactionResponse>> GetUserPaymentHistoryAsync(string userId);
        Task<bool> IsOrderPaidAsync(string orderCode);
        Task ProcessWebhookAsync(SepayWebhookRequest webhookData);
    }
}
