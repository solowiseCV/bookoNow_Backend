namespace BookNow.Application.Interfaces.Services;

public interface IRealTimeChatService
{
    Task PushMessagesReadAsync(Guid userId, object payload, CancellationToken ct);
    Task PushNewMessageAsync(IEnumerable<Guid> userIds, object payload, CancellationToken ct);
}
