namespace BookNow.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Guid identityUserId, string? phoneNumber, string message, CancellationToken ct);
}
