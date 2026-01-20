namespace TravelTechApi.Services.AI
{
    public interface IAIService
    {
        string GetModel();

        Task<string> GenerateTravelPlanAsync(
            string locationName,
            string? currentLocationName,
            int numberOfPeople,
            int duration,
            string priceRange,
            string notes,
            List<string> hobbies,
            List<string> destinationNames
        );
    }
}
