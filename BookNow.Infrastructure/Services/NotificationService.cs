using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BookNow.Infrastructure.Services;

public class NotificationService(
    IUnitOfWork unitOfWork,
    ISmsService smsService,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendNotificationAsync(Guid identityUserId, string? phoneNumber, string message, CancellationToken ct)
    {
        try
        {
            // 1. In-App Notification
            var notification = new Notification(identityUserId, message);
            await unitOfWork.Notifications.AddAsync(notification, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // 2. SMS Notification
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                await smsService.SendSmsAsync(phoneNumber, message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification to User {UserId}", identityUserId);
            // Optionally decide if this should throw or just log.
        }
    }
}
