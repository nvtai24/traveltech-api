using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Giftcode
{
    public class CreateGiftcodeDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; }
    }
}
