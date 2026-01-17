namespace TravelTechApi.Common.Settings
{
    /// <summary>
    /// Email service configuration settings
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Email provider type: Smtp, SendGrid, MailGun
        /// </summary>
        public string Provider { get; set; } = "Smtp";

        /// <summary>
        /// Frontend URL for email confirmation links
        /// </summary>
        public string FrontendUrl { get; set; } = string.Empty;

        /// <summary>
        /// From email address
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// From name displayed in email
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        // SMTP Settings
        /// <summary>
        /// SMTP server host
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// SMTP username
        /// </summary>
        public string SmtpUsername { get; set; } = string.Empty;

        /// <summary>
        /// SMTP password
        /// </summary>
        public string SmtpPassword { get; set; } = string.Empty;

        /// <summary>
        /// Enable SSL/TLS for SMTP
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        // SendGrid Settings
        /// <summary>
        /// SendGrid API key
        /// </summary>
        public string SendGridApiKey { get; set; } = string.Empty;

        // MailGun Settings
        /// <summary>
        /// MailGun API key
        /// </summary>
        public string MailGunApiKey { get; set; } = string.Empty;

        /// <summary>
        /// MailGun domain
        /// </summary>
        public string MailGunDomain { get; set; } = string.Empty;
    }
}
