using BookNow.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BookNow.Infrastructure.Services;

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string message)
    {
        // In a real-world scenario, you would integrate with Twilio, AWS SNS, etc.
        // For development, we log the message to the console.
        logger.LogInformation("==========================================");
        logger.LogInformation("MOCK SMS SENT TO: {PhoneNumber}", phoneNumber);
        logger.LogInformation("MESSAGE: {Message}", message);
        logger.LogInformation("==========================================");
        
        return Task.CompletedTask;
    }
}
