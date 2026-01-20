using TravelTechApi.DTOs.Plan;

namespace TravelTechApi.Services.Plan
{
    public interface IPlanGenerationService
    {
        Task<PlanResponse> GeneratePlanAsync(GeneratePlanRequest request, string userId);
        Task<PlanResponse?> GetPlanByIdAsync(int planId, string userId);
        Task<List<PlanResponse>> GetUserPlansAsync(string userId, int page = 1, int pageSize = 10);
        Task<bool> SavePlanAsync(int planId, string userId);
        Task<bool> DeletePlanAsync(int planId, string userId);
    }
}
