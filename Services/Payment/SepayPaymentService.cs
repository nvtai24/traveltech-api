using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelTechApi.Common;
using TravelTechApi.DTOs.Common;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Common.Settings;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Payment;
using TravelTechApi.Entities;
using TravelTechApi.Services.Giftcode;
using UserPlanSubscriptionEntity = TravelTechApi.Entities.UserPlanSubscription;

namespace TravelTechApi.Services.Payment
{
    public class SepayPaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SepaySettings _sepaySettings;
        private readonly IGiftcodeService _giftcodeService;
        private readonly ILogger<SepayPaymentService> _logger;

        public SepayPaymentService(
            ApplicationDbContext context,
            IMapper mapper,
            IOptions<SepaySettings> sepaySettings,
            IGiftcodeService giftcodeService,
            ILogger<SepayPaymentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _sepaySettings = sepaySettings.Value;
            _giftcodeService = giftcodeService;
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

                // Calculate amount and resolve giftcode first
                decimal amount = plan.Price;
                decimal originalAmount = plan.Price;
                int? giftcodeId = null;

                // Apply Giftcode
                if (!string.IsNullOrEmpty(dto.Giftcode))
                {
                    var isValid = await _giftcodeService.ValidateGiftcodeAsync(dto.Giftcode);
                    if (isValid)
                    {
                        var giftcode = await _giftcodeService.GetGiftcodeByCodeAsync(dto.Giftcode);
                        if (giftcode != null)
                        {
                            var discountAmount = amount * giftcode.DiscountPercentage / 100;

                            if (discountAmount > giftcode.MaxDiscountAmount)
                            {
                                discountAmount = giftcode.MaxDiscountAmount;
                            }

                            amount -= discountAmount;
                            if (amount < 0) amount = 0;
                            giftcodeId = giftcode.Id;
                        }
                    }
                }

                // Check for existing pending transaction for this user and plan
                // Must be pending AND not expired
                var existingTransaction = await _context.PaymentTransactions
                    .Where(p => p.UserId == userId &&
                                p.SubscriptionPlanId == dto.SubscriptionPlanId &&
                                p.Status == PaymentStatus.Pending)
                    .OrderByDescending(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (existingTransaction != null)
                {
                    // Check if expired
                    if (existingTransaction.ExpiresAt.HasValue && existingTransaction.ExpiresAt.Value < DateTime.UtcNow)
                    {
                        // Expired: Mark as Cancelled (or Failed) and create new
                        existingTransaction.Status = PaymentStatus.Expired;
                        // Continue to create new one
                    }
                    else
                    {
                        // Valid pending transaction found
                        // If details match (same amount/giftcode), reuse it
                        if (existingTransaction.Amount == amount && existingTransaction.GiftcodeId == giftcodeId)
                        {
                            string reusedQrUrl = string.Empty;
                            if (existingTransaction.Amount > 0)
                            {
                                reusedQrUrl = GenerateQRCodeUrl(
                                    existingTransaction.Amount,
                                    existingTransaction.OrderCode
                                );
                            }

                            return new PaymentOrderResponse
                            {
                                PaymentId = existingTransaction.Id,
                                OrderCode = existingTransaction.OrderCode,
                                Amount = existingTransaction.Amount,
                                AccountNumber = _sepaySettings.AccountNumber,
                                AccountName = _sepaySettings.AccountName,
                                BankCode = _sepaySettings.BankCode,
                                Description = existingTransaction.OrderCode,
                                QRCodeUrl = reusedQrUrl,
                                Status = existingTransaction.Status,
                                CreatedAt = existingTransaction.CreatedAt
                            };
                        }
                        else
                        {
                            // Details changed, cancel old pending order
                            existingTransaction.Status = PaymentStatus.Cancelled;
                            // Continue to create new one
                        }
                    }
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
                    Amount = amount,
                    OriginalAmount = originalAmount,
                    GiftcodeId = giftcodeId,
                    // PaymentMethod = dto.PaymentMethod,
                    Status = amount <= 0 ? PaymentStatus.Completed : PaymentStatus.Pending,
                    Description = orderCode, // Important: SePay matches based on this content
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15) // Expires in 15 minutes
                };

                // If amount is 0, activate subscription immediately
                if (amount <= 0)
                {
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.TransactionDate = DateTime.UtcNow;
                    payment.Content = "Free Giftcode";

                    var newSub = new UserPlanSubscriptionEntity
                    {
                        UserId = payment.UserId,
                        SubscriptionPlanId = payment.SubscriptionPlanId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(30)
                    };
                    await _context.UserPlanSubscriptions.AddAsync(newSub);

                    if (giftcodeId.HasValue)
                    {
                        await _giftcodeService.IncrementUsageAsync(giftcodeId.Value);
                    }
                }

                await _context.PaymentTransactions.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Generate QR Code URL only if amount > 0
                string qrUrl = string.Empty;
                if (amount > 0)
                {
                    qrUrl = GenerateQRCodeUrl(
                        payment.Amount,
                        orderCode
                    );
                }

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
                    .Include(p => p.Giftcode)
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
                    OriginalAmount = payment.OriginalAmount,
                    Giftcode = payment.Giftcode?.Code,
                    // PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt,
                    TransactionDate = payment.TransactionDate
                };

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment");
                throw new Exception("Failed to get payment details");
            }
        }

        public async Task<PagedResult<PaymentTransactionResponse>> GetUserPaymentHistoryAsync(string userId, int page, int pageSize)
        {
            try
            {
                var query = _context.PaymentTransactions
                    .Include(p => p.SubscriptionPlan)
                    .Include(p => p.Giftcode)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt);

                var totalCount = await query.CountAsync();

                var payments = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = payments.Select(p => new PaymentTransactionResponse
                {
                    Id = p.Id,
                    OrderCode = p.OrderCode,
                    SubscriptionPlanName = p.SubscriptionPlan.Name,
                    Amount = p.Amount,
                    OriginalAmount = p.OriginalAmount,
                    Giftcode = p.Giftcode?.Code,
                    // PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    TransactionDate = p.TransactionDate
                }).ToList();

                return PagedResult<PaymentTransactionResponse>.Create(dtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history");
                throw new Exception("Failed to retrieve history");
            }
        }

        public async Task<bool> IsOrderPaidAsync(string orderCode)
        {
            var payment = await _context.PaymentTransactions
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);

            return payment != null && payment.Status == PaymentStatus.Completed;
        }

        public async Task ProcessWebhookAsync(SepayWebhookRequest webhookData)
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
                _logger.LogInformation("Payment already completed for OrderCode: {OrderCode}", payment.OrderCode);
                return; // Idempotent: already processed
            }

            // 2. Validate Amount
            if (payment.Amount != webhookData.TransferAmount)
            {
                _logger.LogWarning("Amount mismatch. Expected {Exp}, Got {Got}", payment.Amount, webhookData.TransferAmount);
                throw new BadRequestException($"Amount mismatch. Expected {payment.Amount}, got {webhookData.TransferAmount}");
            }

            // 3. Mark as Completed
            payment.Status = PaymentStatus.Completed;
            payment.TransactionId = webhookData.Id;
            payment.TransactionDate = DateTime.SpecifyKind(DateTime.Parse(webhookData.TransactionDate), DateTimeKind.Utc);
            payment.Gateway = webhookData.Gateway;
            payment.AccountNumber = webhookData.AccountNumber;
            payment.Content = webhookData.Content;
            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 4. Activate Subscription
            var newSub = new UserPlanSubscriptionEntity
            {
                UserId = payment.UserId,
                SubscriptionPlanId = payment.SubscriptionPlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30) // Default 30 days
            };

            await _context.UserPlanSubscriptions.AddAsync(newSub);

            // 5. Increment Giftcode Usage
            if (payment.GiftcodeId.HasValue)
            {
                await _giftcodeService.IncrementUsageAsync(payment.GiftcodeId.Value);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment completed successfully for OrderCode: {OrderCode}, TransactionId: {TransactionId}",
                payment.OrderCode, webhookData.Id);
        }


        private string GenerateQRCodeUrl(decimal amount, string content)
        {
            // Using Sepay Quick Link
            // https://qr.sepay.vn/img?acc=SO_TAI_KHOAN&bank=NGAN_HANG&amount=SO_TIEN&des=NOI_DUNG&template=TEMPLATE
            var template = "compact2";
            var encodedContent = Uri.EscapeDataString(content);
            return $"{_sepaySettings.QRCodeBaseUrl}?acc={_sepaySettings.AccountNumber}&bank={_sepaySettings.BankCode}&amount={amount}&des={encodedContent}&template={template}";
        }
    }
}
