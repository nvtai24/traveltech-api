namespace TravelTechApi.Services.Cloudinary
{
    /// <summary>
    /// Result of Cloudinary upload operation
    /// </summary>
    public class CloudinaryUploadResult
    {
        /// <summary>
        /// Public ID of the uploaded image
        /// </summary>
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// HTTP URL of the uploaded image
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// HTTPS URL of the uploaded image
        /// </summary>
        public string SecureUrl { get; set; } = string.Empty;

        /// <summary>
        /// Width of the uploaded image in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the uploaded image in pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Format of the uploaded image (e.g., jpg, png)
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Size of the uploaded image in bytes
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Resource type of the uploaded image
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the image was uploaded
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
