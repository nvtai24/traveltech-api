using System;
using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.SpinPrize
{
    public class UpdateSpinPrizeDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
