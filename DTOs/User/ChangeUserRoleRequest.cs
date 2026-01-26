using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.User
{
    /// <summary>
    /// Request to change user role
    /// </summary>
    public class ChangeUserRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
