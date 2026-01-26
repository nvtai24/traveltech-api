using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Giftcode
{
    public class CreateGiftcodeRequest
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Range(0, 100)]
        public int DiscountPercentage { get; set; }
        [Range(0, double.MaxValue)]
        public decimal MaximumDiscountAmount { get; set; }

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
