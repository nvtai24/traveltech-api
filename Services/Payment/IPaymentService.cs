using TravelTechApi.Common;
using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.Payment;

namespace TravelTechApi.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentOrderResponse> CreatePaymentOrderAsync(string userId, CreatePaymentRequest dto);
        Task<PaymentTransactionResponse> GetPaymentByIdAsync(Guid paymentId, string userId);
        Task<PagedResult<PaymentTransactionResponse>> GetUserPaymentHistoryAsync(string userId, int page, int pageSize);
        Task<bool> IsOrderPaidAsync(string orderCode);
        Task ProcessWebhookAsync(SepayWebhookRequest webhookData);
    }
}
