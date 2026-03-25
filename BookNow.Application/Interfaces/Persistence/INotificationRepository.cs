using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, CancellationToken ct);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct);
    void Update(Notification notification);
}
