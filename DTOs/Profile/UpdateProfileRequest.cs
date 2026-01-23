using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Profile
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public TravelTechApi.Entities.Gender? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
