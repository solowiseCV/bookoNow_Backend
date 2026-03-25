using BookNow.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BookNow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;

       
        _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        _port = int.Parse(configuration["EmailSettings:Port"] ?? "465");
        _username = configuration["EmailSettings:Username"] ?? throw new ArgumentNullException("Email username not configured");
        _password = configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("Email password not configured");
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? _username;
        _fromName = configuration["EmailSettings:FromName"] ?? "BookNow";
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
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

                    <p>
                        <a href='{resetLink}' 
                           style='padding:10px 15px;background:#007bff;color:white;
                                  text-decoration:none;border-radius:5px;display:inline-block;'>
                           Reset Password
                        </a>
                    </p>

                    <p>If you didn't request this, please ignore this email.</p>
                    <p>This link expires in 24 hours.</p>

                    <br/>
                    <p>Best regards,<br/>BookNow Team</p>
                "
            };

            using var client = new SmtpClient();

            client.Timeout = 15000; 

            _logger.LogInformation("SMTP: {Server}, User: {User}", _smtpServer, _username);

           
            await client.ConnectAsync(_smtpServer, _port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw;
        }
    }
}