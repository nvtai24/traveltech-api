using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TravelTechApi.Common.Settings;
using TravelTechApi.DTOs.Contact;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Email
{
    /// <summary>
    /// SMTP implementation of email service using MailKit
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<SmtpEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string userId, string token)
        {
            _logger.LogInformation("Sending email confirmation to: {Email}", email);

            var confirmationUrl = $"{_emailSettings.FrontendUrl}/confirm-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Chào mừng đến với TravelTech!</h1>
        </div>
        <div class='content'>
            <h2>Xác nhận địa chỉ email của bạn</h2>
            <p>Cảm ơn bạn đã đăng ký tài khoản TravelTech. Để hoàn tất quá trình đăng ký, vui lòng xác nhận địa chỉ email của bạn bằng cách nhấp vào nút bên dưới:</p>
            
            <div style='text-align: center;'>
                <a href='{confirmationUrl}' class='button'>Xác nhận Email</a>
            </div>
            
            <p>Hoặc copy link sau vào trình duyệt:</p>
            <p style='background: #fff; padding: 10px; border-left: 4px solid #667eea; word-break: break-all;'>{confirmationUrl}</p>
            
            <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 24 giờ.</p>
            
            <p>Nếu bạn không tạo tài khoản này, vui lòng bỏ qua email này.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 TravelTech. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, "Xác nhận đăng ký tài khoản TravelTech", htmlBody);
            _logger.LogInformation("Email confirmation sent successfully to: {Email}", email);
        }

        public async Task SendPasswordResetEmailAsync(string email, string userId, string token)
        {
            _logger.LogInformation("Sending password reset email to: {Email}", email);

            var resetUrl = $"{_emailSettings.FrontendUrl}/reset-password?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Đặt lại mật khẩu</h1>
        </div>
        <div class='content'>
            <h2>Yêu cầu đặt lại mật khẩu</h2>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản TravelTech của bạn.</p>
            <p>Vui lòng nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
            
            <div style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Đặt lại Mật khẩu</a>
            </div>
            
            <p>Hoặc copy link sau vào trình duyệt:</p>
            <p style='background: #fff; padding: 10px; border-left: 4px solid #667eea; word-break: break-all;'>{resetUrl}</p>
            
            <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau vài giờ.</p>
            
            <p>Nếu bạn không yêu cầu thay đổi mật khẩu, vui lòng bỏ qua email này. Tài khoản của bạn vẫn an toàn.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 TravelTech. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, "Đặt lại mật khẩu TravelTech", htmlBody);
            _logger.LogInformation("Password reset email sent successfully to: {Email}", email);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                _logger.LogDebug("Connecting to SMTP server: {Host}:{Port}", _emailSettings.SmtpHost, _emailSettings.SmtpPort);

                await client.ConnectAsync(
                    _emailSettings.SmtpHost,
                    _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to: {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {To}", to);
                throw;
            }
        }

        public async Task SendEmailContactAsync(string companyEmail, ContactMessage contactMessage)
        {
            var subject = $"New Contact Message: {contactMessage.FullName} - {contactMessage.ContactTopic.Name}";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; border-radius: 0 0 10px 10px; }}
        .field {{ margin-bottom: 15px; }}
        .label {{ font-weight: bold; color: #555; }}
        .value {{ margin-top: 5px; padding: 10px; background-color: white; border-left: 3px solid #4CAF50; }}
        .message-box {{ margin-top: 10px; padding: 15px; background-color: white; border: 1px solid #ddd; min-height: 100px; white-space: pre-wrap; }}
        .footer {{ margin-top: 20px; padding: 15px; text-align: center; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>📧 New Contact Message Received</h2>
        </div>
        <div class='content'>
            <div class='field'>
                <div class='label'>Full Name:</div>
                <div class='value'>{contactMessage.FullName}</div>
            </div>
            <div class='field'>
                <div class='label'>Email:</div>
                <div class='value'><a href='mailto:{contactMessage.Email}'>{contactMessage.Email}</a></div>
            </div>
            <div class='field'>
                <div class='label'>Phone Number:</div>
                <div class='value'><a href='tel:{contactMessage.PhoneNumber}'>{contactMessage.PhoneNumber}</a></div>
            </div>
            <div class='field'>
                <div class='label'>Message:</div>
                <div class='message-box'>{contactMessage.Message}</div>
            </div>
            <div class='field'>
                <div class='label'>Submitted at:</div>
                <div class='value'>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</div>
            </div>
        </div>
        <div class='footer'>
            <p>This is an automated message from TravelTech Contact Form</p>
            <p>Please respond directly to the customer's email: <a href='mailto:{contactMessage.Email}'>{contactMessage.Email}</a></p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(companyEmail, subject, htmlBody);
            _logger.LogInformation("Contact form email sent to company from: {CustomerEmail}", contactMessage.Email);
        }
    }
}
