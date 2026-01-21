using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Contact;
using TravelTechApi.Entities;
using TravelTechApi.Services.Email;

namespace TravelTechApi.Services.Contact;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    private IConfiguration configuration;


    public ContactService(ApplicationDbContext context, IEmailService emailService, IMapper mapper, IConfiguration configuration)
    {
        _emailService = emailService;
        _context = context;
        _mapper = mapper;
        this.configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(ContactMessageRequest contactMessageRequest)
    {
        var contactTopic = _context.ContactTopics.FirstOrDefault(ct => ct.Id == contactMessageRequest.ContactTopicId);
        if (contactTopic == null)
        {
            throw new BadRequestException("Topic not found");
        }

        var contactMessage = _mapper.Map<ContactMessage>(contactMessageRequest);
        contactMessage.ContactTopic = contactTopic;
        _context.ContactMessages.Add(contactMessage);
        var check = await _context.SaveChangesAsync();
        if (check > 0)
        {
            var companyEmail = configuration.GetSection("EmailSettings").GetValue<string>("CompanyEmail") ?? "traveltech.vn.ai@gmail.com";
            await _emailService.SendEmailContactAsync(companyEmail, contactMessage);
            return true;
        }
        return false;
    }
}