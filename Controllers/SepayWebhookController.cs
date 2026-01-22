using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            _logger.LogInformation("Received webhook request: {Request}", dto);

            var apiKeyHeader = Request.Headers["Authorization"].FirstOrDefault();
            var expected = _sepaySettings.WebhookApiKey;

            if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != expected)
            {
                _logger.LogInformation("Invalid API key: {ApiKey}", apiKeyHeader);
                return Unauthorized(new { success = false, message = "Invalid API key" });
            }

            // Webhook endpoint should return 200 quickly to acknowledge receipt
            var result = await _paymentService.ProcessWebhookAsync(dto);

            if (result) return Ok(new { success = true });

            return BadRequest(new { success = false, message = "Failed to process webhook" });
        }
    }
}
