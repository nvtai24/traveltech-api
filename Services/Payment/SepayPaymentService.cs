using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelTechApi.Common;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Common.Settings;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Payment;
using TravelTechApi.Entities;
using UserPlanSubscriptionEntity = TravelTechApi.Entities.UserPlanSubscription;

namespace TravelTechApi.Services.Payment
{
    public class SepayPaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SepaySettings _sepaySettings;
        private readonly ILogger<SepayPaymentService> _logger;

        public SepayPaymentService(
            ApplicationDbContext context,
            IMapper mapper,
            IOptions<SepaySettings> sepaySettings,
            ILogger<SepayPaymentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _sepaySettings = sepaySettings.Value;
            _logger = logger;
        }

        public async Task<PaymentOrderResponse> CreatePaymentOrderAsync(string userId, CreatePaymentRequest dto)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(dto.SubscriptionPlanId);
                if (plan == null)
                {
                    throw new BadRequestException("Invalid subscription plan");
                }

                // Generate unique order code: PLAN{id}U{userIdPrefix}T{timestamp}
                // Shorten userId to keep order code reasonably short if needed, but ensure uniqueness with timestamp
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var shortUserId = userId.Length > 8 ? userId.Substring(0, 8) : userId;
                var orderCode = $"PLAN{dto.SubscriptionPlanId}U{shortUserId}T{timestamp}";

                var payment = new PaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderCode = orderCode,
                    UserId = userId,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    Amount = plan.Price,
                    // PaymentMethod = dto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    Description = orderCode, // Important: SePay matches based on this content
                    CreatedAt = DateTime.UtcNow,
                };

                await _context.PaymentTransactions.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Generate QR Code URL
                var qrUrl = GenerateQRCodeUrl(
                    _sepaySettings.BankCode,
                    _sepaySettings.AccountNumber,
                    _sepaySettings.AccountName,
                    payment.Amount,
                    orderCode
                );

                var response = new PaymentOrderResponse
                {
                    PaymentId = payment.Id,
                    OrderCode = payment.OrderCode,
                    Amount = payment.Amount,
                    AccountNumber = _sepaySettings.AccountNumber,
                    AccountName = _sepaySettings.AccountName,
                    BankCode = _sepaySettings.BankCode,
                    Description = orderCode,
                    QRCodeUrl = qrUrl,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment order");
                throw new Exception("Failed to create payment order");
            }
        }

        public async Task<PaymentTransactionResponse> GetPaymentByIdAsync(Guid paymentId, string userId)
        {
            try
            {
                var payment = await _context.PaymentTransactions
                    .Include(p => p.SubscriptionPlan)
                    .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

                if (payment == null)
                {
                    throw new NotFoundException("Payment not found");
                }

                var dto = new PaymentTransactionResponse
                {
                    Id = payment.Id,
                    OrderCode = payment.OrderCode,
                    SubscriptionPlanName = payment.SubscriptionPlan.Name,
                    Amount = payment.Amount,
                    // PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt,
                    TransactionDate = payment.TransactionDate
                }
                ;

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment");
                throw new Exception("Failed to get payment details");
            }
        }

        public async Task<List<PaymentTransactionResponse>> GetUserPaymentHistoryAsync(string userId)
        {
            try
            {
                var payments = await _context.PaymentTransactions
                    .Include(p => p.SubscriptionPlan)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                var dtos = payments.Select(p => new PaymentTransactionResponse
                {
                    Id = p.Id,
                    OrderCode = p.OrderCode,
                    SubscriptionPlanName = p.SubscriptionPlan.Name,
                    Amount = p.Amount,
                    // PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    TransactionDate = p.TransactionDate
                }).ToList();

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history");
                throw new Exception("Failed to retrieve history");
            }
        }

        public async Task<bool> ProcessWebhookAsync(SepayWebhookRequest webhookData)
        {
            try
            {
                // 1. Find payment by matching OrderCode in content
                // SePay sends content like "PLAN1U123..."
                // Logic: Search for a pending transaction where OrderCode matches (or is contained in) the description
                var contentData = webhookData.Content;

                // Simple strict match first
                var payment = await _context.PaymentTransactions
                    .Include(p => p.SubscriptionPlan)
                    .FirstOrDefaultAsync(p => contentData.Contains(p.OrderCode)); // Can relax this to Contains if needed

                if (payment == null)
                {
                    _logger.LogWarning("No payment found for code: {Code}", contentData);
                    throw new BadRequestException("Payment not found");
                }

                if (payment.Status == PaymentStatus.Completed)
                {
                    return true; // Idempotent: already processed
                }

                // 2. Validate Amount
                if (payment.Amount != webhookData.Amount)
                {
                    _logger.LogWarning("Amount mismatch. Expected {Exp}, Got {Got}", payment.Amount, webhookData.Amount);
                    throw new BadRequestException("Amount mismatch");
                }

                // 3. Mark as Completed
                payment.Status = PaymentStatus.Completed;
                payment.TransactionId = webhookData.TransactionId;
                payment.TransactionDate = DateTime.Parse(webhookData.TransactionDate);
                payment.Gateway = webhookData.Gateway;
                payment.AccountNumber = webhookData.AccountNumber;
                payment.Content = webhookData.Content;
                payment.UpdatedAt = DateTime.UtcNow;

                // 4. Activate Subscription
                var newSub = new UserPlanSubscriptionEntity
                {
                    UserId = payment.UserId,
                    SubscriptionPlanId = payment.SubscriptionPlanId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30) // Default 30 days
                };

                _context.UserPlanSubscriptions.Add(newSub);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                throw new Exception("Internal error processing webhook");
            }
        }

        private string GenerateQRCodeUrl(string bank, string accNo, string accName, decimal amount, string content)
        {
            // Using VietQR Quick Link
            // Format: https://img.vietqr.io/image/<BANK>-<ACC_NO>-<TEMPLATE>.png
            var template = "compact2";
            var encodedName = Uri.EscapeDataString(accName);
            var encodedContent = Uri.EscapeDataString(content);
            return $"{_sepaySettings.QRCodeBaseUrl}/{bank}-{accNo}-{template}.png?amount={amount}&addInfo={encodedContent}&accountName={encodedName}";
        }
    }
}
