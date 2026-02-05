namespace BookNow.Application.Interfaces.Messaging;

public interface IMessageService
{
    Task SendAsync(
        string recipient,
        string subject,
        string body,
        CancellationToken ct);
}
