namespace TravelTechApi.DTOs.Giftcode
{
    public class GiftcodeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
