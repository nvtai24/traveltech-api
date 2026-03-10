using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class SpinPrize
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        // public string ImageUrl { get; set; }
        // public int Quantity { get; set; }
        // public int RemainingQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}