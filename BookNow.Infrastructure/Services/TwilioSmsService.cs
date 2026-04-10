using BookNow.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BookNow.Infrastructure.Services;

public class TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger) : ISmsService
{
    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var accountSid = configuration["Twilio:AccountSid"];
            var authToken = configuration["Twilio:AuthToken"];
            var fromNumber = configuration["Twilio:FromNumber"] ?? "+1234567890"; 

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            {
                logger.LogWarning("Twilio credentials not configured. SMS not sent to {PhoneNumber}", phoneNumber);
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );

            logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
        }
    }
}
