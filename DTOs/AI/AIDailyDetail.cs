namespace TravelTechApi.DTOs.AI
{
    /// <summary>
    /// Response DTOs for Step 2: Daily Detail Generation
    /// </summary>
    public class AIDailyDetailResponse
    {
        public int DayNumber { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<AIActivityDto> Activities { get; set; } = new();
        public List<AIFoodRecommendationDto> FoodRecommendations { get; set; } = new();
    }

    public class AIActivityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? DestinationName { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Tips { get; set; }
        public int Order { get; set; }
        public string? MapUrl { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AIFoodRecommendationDto
    {
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
