namespace BookNow.Application.Interfaces.Services;

public interface IRealTimeNotificationService
{
    Task PushNotificationAsync(Guid userId, object payload, CancellationToken ct);
}
