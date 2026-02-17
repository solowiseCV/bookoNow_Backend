namespace BookNow.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetToken);
}
