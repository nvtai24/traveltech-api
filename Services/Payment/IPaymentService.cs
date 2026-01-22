using TravelTechApi.Common;
using TravelTechApi.DTOs.Payment;

namespace TravelTechApi.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentOrderResponse> CreatePaymentOrderAsync(string userId, CreatePaymentRequest dto);
        Task<PaymentTransactionDto> GetPaymentByIdAsync(Guid paymentId, string userId);
        Task<List<PaymentTransactionDto>> GetUserPaymentHistoryAsync(string userId);
        Task<bool> ProcessWebhookAsync(SepayWebhookRequest webhookData);
    }
}
