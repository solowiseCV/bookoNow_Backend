using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookNow.Presentation.Hubs;

[Authorize]
public sealed class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation("User {UserId} connected to NotificationHub", userId);
        
        // Add user to their own private group for targeted notifications
        if (userId != "unknown")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation("Connection {ConnectionId} for User {UserId} disconnected from NotificationHub. Cleanup handled by SignalR.", Context.ConnectionId, userId);
        await base.OnDisconnectedAsync(exception);
    }
}
