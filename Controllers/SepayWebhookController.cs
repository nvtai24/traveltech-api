using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TravelTechApi.Common;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Common.Settings;
using TravelTechApi.DTOs.Payment;
using TravelTechApi.Services.Payment;

namespace TravelTechApi.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class SepayWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly SepaySettings _sepaySettings;
        private readonly ILogger<SepayWebhookController> _logger;

        public SepayWebhookController(IPaymentService paymentService, IOptions<SepaySettings> sepaySettings, ILogger<SepayWebhookController> logger)
        {
            _paymentService = paymentService;
            _sepaySettings = sepaySettings.Value;
            _logger = logger;
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] SepayWebhookRequest dto)
        {
            _logger.LogInformation("Received webhook request: {Request}", System.Text.Json.JsonSerializer.Serialize(dto));
            var apiKeyHeader = Request.Headers["Authorization"].FirstOrDefault();
            var expected = "Apikey " + _sepaySettings.WebhookApiKey;

            if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != expected)
            {
                _logger.LogInformation("Invalid API key: {ApiKey}", apiKeyHeader);
                return Unauthorized(new { success = false, message = "Invalid API key" });
            }

            // Webhook endpoint should return 200 quickly to acknowledge receipt
            try
            {
                await _paymentService.ProcessWebhookAsync(dto);

                _logger.LogInformation("Webhook processed successfully for transaction: {TransactionId}", dto.Id);
                return Ok(new { success = true, message = "Payment processed successfully" });
            }
            catch (BadRequestException ex)
            {
                // Business logic errors (payment not found, amount mismatch, etc.)
                _logger.LogWarning(ex, "Webhook validation failed: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected errors (database issues, etc.)
                _logger.LogError(ex, "Unexpected error processing webhook for transaction: {TransactionId}", dto.Id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Internal server error processing payment" }
                );
            }
        }
    }
}
