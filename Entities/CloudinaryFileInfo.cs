using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    public class CloudinaryFileInfo
    {
        [Key]
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SecureUrl { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}