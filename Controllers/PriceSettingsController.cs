
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Destination;
using TravelTechApi.Services.Destination;

namespace TravelTechApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceSettingsController : ControllerBase
{
    private readonly IPriceSettingService _priceSettingsService;
    private readonly ILogger<PriceSettingsController> _logger;

    public PriceSettingsController(IPriceSettingService priceSettingsService, ILogger<PriceSettingsController> logger)
    {
        _priceSettingsService = priceSettingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPriceSettings()
    {
        var priceSettings = await _priceSettingsService.GetPriceSettingsAsync();
        return this.Success(priceSettings, "Price settings retrieved successfully");
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePriceSettings(PriceSettingDto priceSettingsDto)
    {
        var isUpdated = await _priceSettingsService.UpdatePriceSettingsAsync(priceSettingsDto);
        if (!isUpdated)
        {
            return this.Failed("Price settings updated failed");
        }
        return this.Success("Price settings updated successfully");
    }
}