namespace TravelTechApi.Services.Interfaces
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send email confirmation to user
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="userId">User ID</param>
        /// <param name="token">Confirmation token</param>
        Task SendEmailConfirmationAsync(string email, string userId, string token);

        /// <summary>
        /// Send generic email
        /// </summary>
        /// <param name="to">Recipient email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="htmlBody">Email HTML body</param>
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
