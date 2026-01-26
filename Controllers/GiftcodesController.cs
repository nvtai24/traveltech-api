using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Giftcode;
using TravelTechApi.Services.Giftcode;

namespace TravelTechApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftcodesController : ControllerBase
    {
        private readonly IGiftcodeService _giftcodeService;
        private readonly ILogger<GiftcodesController> _logger;

        public GiftcodesController(IGiftcodeService giftcodeService, ILogger<GiftcodesController> logger)
        {
            _giftcodeService = giftcodeService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _giftcodeService.GetAllGiftcodesAsync(page, pageSize);
            return this.Success(result, "Retrieved all giftcodes successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _giftcodeService.GetGiftcodeByIdAsync(id);
            if (result == null)
            {
                return this.NotFound("Giftcode not found");
            }
            return this.Success(result, "Retrieved giftcode successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateGiftcodeRequest dto)
        {
            if (await _giftcodeService.GetGiftcodeByCodeAsync(dto.Code) != null)
            {
                return this.BadRequest("Giftcode with this code already exists");
            }

            var result = await _giftcodeService.CreateGiftcodeAsync(dto);
            // Note: ControllerExtensions might not have CreatedAtAction wrapper, so we use Success which returns 200 OK.
            // If strict 201 is needed, we might need to check extensions or use standard CreatedAtAction, 
            // but user asked for "values I defined", implying the extension methods.
            // DestinationsController uses this.Success for creation too.
            return this.Created(result, "Giftcode created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGiftcodeRequest dto)
        {
            var result = await _giftcodeService.UpdateGiftcodeAsync(id, dto);
            if (result == null)
            {
                return this.NotFound("Giftcode not found");
            }
            return this.Success(result, "Giftcode updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _giftcodeService.DeleteGiftcodeAsync(id);
            if (!success)
            {
                return this.NotFound("Giftcode not found");
            }
            return this.Success("Giftcode deleted successfully");
        }

        [HttpGet("validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var isValid = await _giftcodeService.ValidateGiftcodeAsync(code);
            if (!isValid)
            {
                return this.BadRequest("Invalid or expired giftcode");
            }

            var giftcode = await _giftcodeService.GetGiftcodeByCodeAsync(code);
            return this.Success(giftcode, "Giftcode is valid");
        }
    }
}
