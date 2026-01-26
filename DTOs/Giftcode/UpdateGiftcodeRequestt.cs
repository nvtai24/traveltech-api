using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.DTOs.Giftcode
{
    public class UpdateGiftcodeRequest
    {
        [Range(0, 100)]
        public int DiscountPercentage { get; set; }
        [Range(0, double.MaxValue)]
        public decimal MaximumDiscountAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
    }
}
