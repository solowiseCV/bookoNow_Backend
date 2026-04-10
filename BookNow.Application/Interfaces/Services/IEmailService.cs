namespace BookNow.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetToken);
    Task SendEmailAsync(string to, string subject, string body);
    Task SendNotificationEmailAsync(string to, string subject, string title, string message, string? buttonText = null, string? buttonUrl = null);
}
