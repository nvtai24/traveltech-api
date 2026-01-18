namespace TravelTechApi.DTOs
{
    /// <summary>
    /// DTO for Destination entity
    /// </summary>
    public class DestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string? VideoUrl { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
