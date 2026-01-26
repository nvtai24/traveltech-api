using System.ComponentModel.DataAnnotations;
using TravelTechApi.Entities;

namespace TravelTechApi.DTOs.User
{
    /// <summary>
    /// Request to update user information (admin)
    /// </summary>
    public class UpdateUserRequest
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Dob { get; set; }
    }
}
