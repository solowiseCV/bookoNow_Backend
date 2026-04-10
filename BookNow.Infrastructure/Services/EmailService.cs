using BookNow.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BookNow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IHostEnvironment _env;
    private readonly string _smtpServer;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
        
        _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        _port = int.Parse(configuration["EmailSettings:Port"] ?? "465");
        _username = configuration["EmailSettings:Username"] ?? throw new ArgumentNullException("Email username not configured");
        _password = configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("Email password not configured");
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? _username;
        _fromName = configuration["EmailSettings:FromName"] ?? "BookNow";
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        await SendNotificationEmailAsync(
            email,
            "Password Reset - BookNow",
            "Password Reset Request",
            "You requested a password reset for your BookNow account. This link expires in 24 hours.",
            "Reset Password",
            resetLink
        );
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            client.Timeout = 15000;

            await client.ConnectAsync(_smtpServer, _port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendNotificationEmailAsync(string to, string subject, string title, string message, string? buttonText = null, string? buttonUrl = null)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Email", "GenericTemplate.html");
        _logger.LogInformation("Attempting to load template from: {Path}", templatePath);
        var genericTemplate = await File.ReadAllTextAsync(templatePath);

        var buttonHtml = string.Empty;
        if (!string.IsNullOrEmpty(buttonUrl) && !string.IsNullOrEmpty(buttonText))
        {
            buttonHtml = $@"<div style='text-align: center;'>
                                <a href='{buttonUrl}' class='button'>{buttonText}</a>
                            </div>";
        }

        var content = genericTemplate
            .Replace("{{Title}}", title)
            .Replace("{{Message}}", message)
            .Replace("{{ButtonHtml}}", buttonHtml);

        var fullBody = await WrapWithBaseLayoutAsync(content);
        await SendEmailAsync(to, subject, fullBody);
    }

    private async Task<string> WrapWithBaseLayoutAsync(string content)
    {
        var layoutPath = Path.Combine(_env.ContentRootPath, "Templates", "Email", "BaseLayout.html");
        var layout = await File.ReadAllTextAsync(layoutPath);

        return layout
            .Replace("{{MainContent}}", content)
            .Replace("{{Year}}", DateTime.Now.Year.ToString());
    }
}