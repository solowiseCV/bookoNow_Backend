using System.Linq;
using BookNow.Application.Interfaces.Services;
using BookNow.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BookNow.Presentation.Services;

public class RealTimeChatService(IHubContext<ChatHub> hubContext) : IRealTimeChatService
{
    public async Task PushMessagesReadAsync(Guid userId, object payload, CancellationToken ct)
        => await hubContext.Clients.Group($"user-{userId}")
            .SendAsync("MessagesRead", payload, cancellationToken: ct);

    public async Task PushNewMessageAsync(IEnumerable<Guid> userIds, object payload, CancellationToken ct)
        => await hubContext.Clients.Groups(userIds.Select(id => $"user-{id}"))
            .SendAsync("NewMessage", payload, cancellationToken: ct);
}
