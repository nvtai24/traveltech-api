namespace TravelTechApi.Common.Settings
{
    /// <summary>
    /// JWT settings configuration
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key for signing JWT tokens
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Access token expiration in minutes
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; } = 15;

        /// <summary>
        /// Refresh token expiration in days
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
