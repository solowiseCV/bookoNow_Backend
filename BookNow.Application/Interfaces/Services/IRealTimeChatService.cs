namespace BookNow.Application.Interfaces.Services;

public interface IRealTimeChatService
{
    Task PushMessagesReadAsync(Guid userId, object payload, CancellationToken ct);
}
