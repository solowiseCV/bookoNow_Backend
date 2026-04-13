using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookNow.Presentation.Hubs;

[Authorize]
public sealed class ChatHub(ILogger<ChatHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation("User {UserId} connected to ChatHub", userId);

        if (userId != "unknown")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation("Connection {ConnectionId} for User {UserId} disconnected from ChatHub", Context.ConnectionId, userId);
        await base.OnDisconnectedAsync(exception);
    }
}
