using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class Giftcode
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        [Range(0, 100)]
        public int DiscountPercentage { get; set; }

        public decimal MaximumDiscountAmount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public int UsageLimit { get; set; }

        public int UsageCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}