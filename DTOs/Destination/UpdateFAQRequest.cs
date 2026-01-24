using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Destination
{
    public class UpdateFAQRequest
    {
        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;
    }
}
