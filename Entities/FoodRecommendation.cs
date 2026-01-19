using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class FoodRecommendation
    {
        [Key]
        public int Id { get; set; }

        public int DailyItineraryId { get; set; }
        public DailyItinerary DailyItinerary { get; set; } = null!;

        public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner, Snack
        public string DishName { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Price range
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public string? Description { get; set; }
        public string? SpecialtyNote { get; set; }
        public string? ImageUrl { get; set; }
        public string? MapUrl { get; set; }
    }
}
