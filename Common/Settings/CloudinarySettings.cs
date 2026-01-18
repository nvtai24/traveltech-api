namespace TravelTechApi.Common.Settings
{
    /// <summary>
    /// Cloudinary configuration settings
    /// </summary>
    public class CloudinarySettings
    {
        /// <summary>
        /// Cloudinary cloud name
        /// </summary>
        public string CloudName { get; set; } = string.Empty;

        /// <summary>
        /// Cloudinary API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Cloudinary API secret
        /// </summary>
        public string ApiSecret { get; set; } = string.Empty;
    }
}
