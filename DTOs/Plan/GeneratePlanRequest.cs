using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Plan
{
    public class GeneratePlanRequest
    {
        [Required]
        public int LocationId { get; set; } // Địa điểm đến

        public int? CurrentLocationId { get; set; } // Nơi ở hiện tại (optional)

        [Required]
        [Range(1, 100)]
        public int NumberOfPeople { get; set; }

        [Required]
        [Range(1, 30)]
        public int Duration { get; set; }

        [Required]
        public int PriceSettingId { get; set; }

        public List<int> HobbyIds { get; set; } = new();

        public string? Notes { get; set; }
    }
}
