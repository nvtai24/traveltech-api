using Microsoft.AspNetCore.Mvc;
using TravelTechApi.DTOs.Payment;
using TravelTechApi.Services.Payment;

namespace TravelTechApi.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class SepayWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public SepayWebhookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] SepayWebhookRequest dto)
        {
            // Webhook endpoint should return 200 quickly to acknowledge receipt
            var result = await _paymentService.ProcessWebhookAsync(dto);

            if (result) return Ok(new { success = true });

            return BadRequest(new { success = false, message = "Failed to process webhook" });
        }
    }
}
