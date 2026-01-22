using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Payment;
using TravelTechApi.Services.Payment;

namespace TravelTechApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // dto.PaymentMethod = Entities.PaymentMethod.BankTransfer; // Force bank transfer for now
            var result = await _paymentService.CreatePaymentOrderAsync(userId, dto);

            return this.Success(result, "Payment order created successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return this.NotFound("User not found");

            var result = await _paymentService.GetPaymentByIdAsync(id, userId);

            return this.Success(result, "Payment details retrieved successfully");
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return this.Unauthorized();

            var result = await _paymentService.GetUserPaymentHistoryAsync(userId);
            return this.Success(result, "Payment history retrieved successfully");
        }
    }
}
