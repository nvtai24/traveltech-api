using System;

namespace TravelTechApi.DTOs.SpinPrize
{
    public class SpinPrizeConfigDto
    {
        public bool IsEnabled { get; set; } = true;

        public List<SpinPrizeItemDto> Prizes { get; set; } = new List<SpinPrizeItemDto>();
        public bool IsShuffled { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
