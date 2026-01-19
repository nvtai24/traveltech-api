namespace TravelTechApi.DTOs.Plan
{
    public class FoodRecommendationResponse
    {
        public int Id { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string DishName { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Description { get; set; }
        public string? SpecialtyNote { get; set; }
        public string? ImageUrl { get; set; }
        public string? MapUrl { get; set; }
    }
}
