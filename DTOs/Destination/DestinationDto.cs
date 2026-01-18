namespace TravelTechApi.DTOs
{
    /// <summary>
    /// DTO for Destination entity
    /// </summary>
    public class DestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string? LocationName { get; set; }
        public string? ThumbnailUrl { get; set; } = string.Empty;
    }

    public class DestinationDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string History { get; set; } = string.Empty;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string? VideoUrl { get; set; }
        public string? LocationName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<FaqDto> FAQs { get; set; } = new List<FaqDto>();
    }

    public class FaqDto
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class DestinationSharingDto
    {
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}
