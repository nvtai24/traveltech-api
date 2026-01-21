using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Contact;
using TravelTechApi.Entities;
using TravelTechApi.Services.Contact;

namespace TravelTechApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmailAsync([FromBody] ContactMessageRequest contactMessageRequest)
    {
        var isSent = await _contactService.SendEmailAsync(contactMessageRequest);
        if (isSent)
        {
            return this.Success("Send email successfully");
        }
        return this.InternalServerError("Send email failed");
    }

    [HttpGet("topics")]
    public async Task<IActionResult> GetTopicsAsync()
    {
        var topics = await _contactService.GetTopicsAsync();
        return this.Success(topics);
    }
}