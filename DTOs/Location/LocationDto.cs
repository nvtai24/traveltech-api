namespace TravelTechApi.DTOs
{
    /// <summary>
    /// DTO for Location entity
    /// </summary>
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public string? RegionName { get; set; }
    }
}
