namespace TravelTechApi.DTOs.Giftcode
{
    public class GiftcodeResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int DiscountPercentage { get; set; }
        public decimal MaximumDiscountAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
