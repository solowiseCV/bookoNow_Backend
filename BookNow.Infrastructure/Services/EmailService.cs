using BookNow.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BookNow.Infrastructure.Services;

public class EmailService(ILogger<EmailService> logger, IConfiguration configuration) : IEmailService
{
    private readonly string _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
    private readonly int _port = int.Parse(configuration["EmailSettings:Port"] ?? "587");
    private readonly string _username = configuration["EmailSettings:Username"] ?? "";
    private readonly string _password = configuration["EmailSettings:Password"] ?? "";
    private readonly string _fromEmail = configuration["EmailSettings:FromEmail"] ?? "noreply@yourapp.com";
    private readonly string _fromName = configuration["EmailSettings:FromName"] ?? "BookNow App";

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset - BookNow";
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Password Reset Request</h2>
                    <p>You requested a password reset for your BookNow account.</p>
                    <p>Use the following token to reset your password:</p>
                    <p><strong>{resetToken}</strong></p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>This token expires in 24 hours.</p>
                    <br>
                    <p>Best regards,<br>BookNow Team</p>
                "
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw; // Re-throw so the caller can handle it
        }
    }
}