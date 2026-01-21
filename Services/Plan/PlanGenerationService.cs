using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Plan;
using TravelTechApi.Entities;
using TravelTechApi.Services.AI;
using TravelTechApi.Services.UserPlanSubscription;

namespace TravelTechApi.Services.Plan
{
    public class PlanGenerationService : IPlanGenerationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly IMapper _mapper;
        private readonly ILogger<PlanGenerationService> _logger;

        private readonly IUserPlanSubscriptionService _userPlanSubscriptionService;

        public PlanGenerationService(
            ApplicationDbContext context,
            IAIService aiService,
            IMapper mapper,
            ILogger<PlanGenerationService> logger,
            IUserPlanSubscriptionService userPlanSubscriptionService)
        {
            _context = context;
            _aiService = aiService;
            _mapper = mapper;
            _logger = logger;
            _userPlanSubscriptionService = userPlanSubscriptionService;
        }

        public async Task<PlanResponse> GeneratePlanAsync(GeneratePlanRequest request, string userId)
        {
            var isLimited = await _userPlanSubscriptionService.IsPlanLimitedAsync(userId);
            if (isLimited)
            {
                throw new InvalidOperationException("You have reached the maximum number of plans allowed for your subscription level.");
            }

            try
            {
                _logger.LogInformation("Generating travel plan for user {UserId}, location {LocationId}", userId, request.LocationId);

                // 1. Validate and fetch data
                var location = await _context.Locations
                    .Include(l => l.Destinations)
                    .FirstOrDefaultAsync(l => l.Id == request.LocationId)
                    ?? throw new NotFoundException("Location not found");

                Location? currentLocation = null;
                if (request.CurrentLocationId.HasValue)
                {
                    currentLocation = await _context.Locations
                        .FirstOrDefaultAsync(l => l.Id == request.CurrentLocationId.Value);
                }

                var priceSetting = await _context.Set<PriceSetting>()
                    .FirstOrDefaultAsync(ps => ps.Id == request.PriceSettingId)
                    ?? throw new NotFoundException("Price setting not found");

                var hobbies = await _context.Set<TravelHobby>()
                    .Where(h => request.HobbyIds.Contains(h.Id))
                    .ToListAsync();

                // 2. Call AI service
                var destinationNames = location.Destinations.Select(d => d.Name).ToList();
                var hobbyNames = hobbies.Select(h => h.Name).ToList();

                var aiResponse = await _aiService.GenerateTravelPlanAsync(
                    location.Name,
                    currentLocation?.Name,
                    request.NumberOfPeople,
                    request.Duration,
                    priceSetting.Name,
                    request.Notes ?? string.Empty,
                    hobbyNames,
                    destinationNames
                );

                // 3. Parse AI response
                var cleanAiResponse = CleanJsonString(aiResponse);
                var aiPlan = ParseAIResponse(cleanAiResponse);

                // 4. Create Plan entity
                var plan = new Entities.Plan
                {
                    LocationId = request.LocationId,
                    CurrentLocationId = request.CurrentLocationId,
                    NumberOfPeople = request.NumberOfPeople,
                    Duration = request.Duration,
                    TotalCostEstimatedFrom = aiPlan.TotalEstimatedCostFrom,
                    TotalCostEstimatedTo = aiPlan.TotalEstimatedCostTo,
                    PriceSettingId = request.PriceSettingId,
                    Hobbies = hobbies,
                    Note = request.Notes ?? string.Empty,
                    IsSaved = false,
                    GeneratedAt = DateTime.UtcNow,
                    AIModel = _aiService.GetModel(),
                    UserId = userId,
                    AIResponseJson = cleanAiResponse
                };

                // 5. Add accommodations
                foreach (var acc in aiPlan.Accommodations)
                {
                    plan.AccommodationRecommendations.Add(new AccommodationRecommendation
                    {
                        AccommodationType = acc.AccommodationType,
                        Name = acc.Name,
                        Address = acc.Address,
                        PricePerNight = acc.PricePerNight,
                        Description = acc.Description,
                        Amenities = acc.Amenities,
                        Rating = acc.Rating,
                        BookingUrl = acc.BookingUrl,
                        ContactInfo = acc.ContactInfo,
                        ImageUrl = acc.ImageUrl,
                        MapUrl = acc.MapUrl
                    });
                }

                // 6. Add transportations
                foreach (var trans in aiPlan.Transportations)
                {
                    plan.TransportationRecommendations.Add(new TransportationRecommendation
                    {
                        TransportationType = trans.TransportationType,
                        Route = trans.Route,
                        PriceFrom = trans.PriceFrom,
                        PriceTo = trans.PriceTo,
                        Duration = trans.Duration,
                        BookingInfo = trans.BookingInfo,
                        Tips = trans.Tips,
                        Provider = trans.Provider
                    });
                }

                // 7. Add daily itineraries
                foreach (var dayPlan in aiPlan.DailyItineraries)
                {
                    var dailyItinerary = new DailyItinerary
                    {
                        DayNumber = dayPlan.DayNumber,
                        Summary = dayPlan.Summary
                    };

                    // Add activities
                    foreach (var act in dayPlan.Activities)
                    {
                        // Try to find matching destination
                        int? destinationId = null;
                        if (!string.IsNullOrEmpty(act.DestinationName))
                        {
                            var destination = location.Destinations
                                .FirstOrDefault(d => d.Name.Contains(act.DestinationName, StringComparison.OrdinalIgnoreCase));
                            destinationId = destination?.Id;
                        }

                        dailyItinerary.Activities.Add(new Activity
                        {
                            Name = act.Name,
                            Description = act.Description,
                            StartTime = TimeSpan.Parse(act.StartTime),
                            EndTime = TimeSpan.Parse(act.EndTime),
                            // DestinationId = destinationId,
                            PriceFrom = act.PriceFrom,
                            PriceTo = act.PriceTo,
                            Tips = act.Tips,
                            Order = act.Order,
                            MapUrl = act.MapUrl
                        });
                    }

                    // Add food recommendations
                    foreach (var food in dayPlan.FoodRecommendations)
                    {
                        dailyItinerary.FoodRecommendations.Add(new FoodRecommendation
                        {
                            MealType = food.MealType,
                            DishName = food.DishName,
                            RestaurantName = food.RestaurantName,
                            Address = food.Address,
                            PriceFrom = food.PriceFrom,
                            PriceTo = food.PriceTo,
                            Description = food.Description,
                            SpecialtyNote = food.SpecialtyNote,
                            ImageUrl = food.ImageUrl,
                            MapUrl = food.MapUrl
                        });
                    }

                    plan.DailyItineraries.Add(dailyItinerary);
                }

                // 8. Save to database
                _context.Plans.Add(plan);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully generated plan {PlanId} for user {UserId}", plan.Id, userId);

                // 9. Return response
                return await GetPlanByIdAsync(plan.Id, userId)
                    ?? throw new Exception("Failed to retrieve generated plan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating travel plan");
                throw;
            }
        }

        public async Task<PlanResponse?> GetPlanByIdAsync(int planId, string userId)
        {
            var plan = await _context.Plans
                .Include(p => p.Location)
                .Include(p => p.CurrentLocation)
                .Include(p => p.PriceSetting)
                .Include(p => p.Hobbies)
                .Include(p => p.AccommodationRecommendations)
                .Include(p => p.TransportationRecommendations)
                .Include(p => p.DailyItineraries)
                    .ThenInclude(d => d.Activities)
                // .ThenInclude(a => a.Destination)
                .Include(p => p.DailyItineraries)
                    .ThenInclude(d => d.FoodRecommendations)
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            return plan == null ? null : _mapper.Map<PlanResponse>(plan);
        }

        public async Task<List<PlanResponse>> GetUserPlansAsync(string userId, int page = 1, int pageSize = 10)
        {
            var plans = await _context.Plans
                .Include(p => p.Location)
                .Include(p => p.CurrentLocation)
                .Include(p => p.PriceSetting)
                .Where(p => p.UserId == userId && p.IsSaved)
                .OrderByDescending(p => p.GeneratedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<PlanResponse>>(plans);
        }

        public async Task<bool> SavePlanAsync(int planId, string userId)
        {
            var plan = await _context.Plans
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            if (plan == null)
                return false;

            plan.IsSaved = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePlanAsync(int planId, string userId)
        {
            var plan = await _context.Plans
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            if (plan == null)
                return false;

            plan.IsSaved = false;
            await _context.SaveChangesAsync();

            return true;
        }

        private string CleanJsonString(string aiResponse)
        {
            var jsonResponse = aiResponse.Trim();
            if (jsonResponse.StartsWith("```json"))
            {
                jsonResponse = jsonResponse.Substring(7);
            }
            else if (jsonResponse.StartsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(3);
            }

            if (jsonResponse.EndsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3);
            }

            return jsonResponse.Trim();
        }

        private AIPlanResponse ParseAIResponse(string jsonResponse)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<AIPlanResponse>(jsonResponse, options)
                    ?? throw new Exception("Failed to parse AI response");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse AI response: {Response}", jsonResponse);
                throw new Exception("Invalid AI response format");
            }
        }

        // Internal DTOs for AI response parsing
        private class AIPlanResponse
        {
            public string Summary { get; set; } = string.Empty;
            public decimal TotalEstimatedCostFrom { get; set; }
            public decimal TotalEstimatedCostTo { get; set; }
            public List<AIAccommodation> Accommodations { get; set; } = new();
            public List<AITransportation> Transportations { get; set; } = new();
            public List<AIDailyItinerary> DailyItineraries { get; set; } = new();
        }

        private class AIAccommodation
        {
            public string AccommodationType { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public decimal PricePerNight { get; set; }
            public string Description { get; set; } = string.Empty;
            public List<string> Amenities { get; set; } = new();
            public decimal? Rating { get; set; }
            public string? BookingUrl { get; set; }
            public string? ContactInfo { get; set; }
            public string? ImageUrl { get; set; }
            public string? MapUrl { get; set; }
        }

        private class AITransportation
        {
            public string TransportationType { get; set; } = string.Empty;
            public string Route { get; set; } = string.Empty;
            public decimal PriceFrom { get; set; }
            public decimal PriceTo { get; set; }
            public string Duration { get; set; } = string.Empty;
            public string? BookingInfo { get; set; }
            public string? Tips { get; set; }
            public string? Provider { get; set; }
        }

        private class AIDailyItinerary
        {
            public int DayNumber { get; set; }
            public string Summary { get; set; } = string.Empty;
            public List<AIActivity> Activities { get; set; } = new();
            public List<AIFoodRecommendation> FoodRecommendations { get; set; } = new();
        }

        private class AIActivity
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
        }

        private class AIFoodRecommendation
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
}
