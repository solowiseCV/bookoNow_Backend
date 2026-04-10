using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BookNow.Infrastructure.Services;

public class NotificationService(
    IUnitOfWork unitOfWork,
    ISmsService smsService,
    IRealTimeNotificationService realTimeNotificationService,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendNotificationAsync(Guid identityUserId, string? phoneNumber, string message, CancellationToken ct)
    {
        try
        {
            //  In-App Notification (Database)
            var notification = new Notification(identityUserId, message);
            await unitOfWork.Notifications.AddAsync(notification, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Real-Time Push (Abstractions)
            await realTimeNotificationService.PushNotificationAsync(identityUserId, new 
            { 
                id = notification.Id,
                message = notification.Message,
                createdAt = notification.CreatedAt,
                isRead = notification.IsRead
            }, ct);

            // 3. SMS Notification (Optional fallback)
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                await smsService.SendSmsAsync(phoneNumber, message);
            }
            
            logger.LogInformation("Notification sent to User {UserId} via RealTime service and DB", identityUserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification to User {UserId}", identityUserId);
        }
    }
}
