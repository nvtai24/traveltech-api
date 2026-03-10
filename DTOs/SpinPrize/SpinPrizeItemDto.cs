using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.SpinPrize
{
    public class SpinPrizeItemDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Icon { get; set; }
    }
}
