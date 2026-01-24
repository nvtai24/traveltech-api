using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Giftcode
{
    public class UpdateGiftcodeDto
    {
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; }

        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; }
    }
}
