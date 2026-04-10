using BookNow.Application.Interfaces.Services;
using BookNow.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BookNow.Presentation.Services;

public class RealTimeNotificationService(IHubContext<NotificationHub> hubContext) : IRealTimeNotificationService
{
    public async Task PushNotificationAsync(Guid userId, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"user-{userId}")
            .SendAsync("ReceiveNotification", payload, cancellationToken: ct);
    }
}
