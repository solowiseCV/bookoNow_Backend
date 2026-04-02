using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class NotificationRepository(BookNowDbContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification, CancellationToken ct)
    {
        await context.Notifications.AddAsync(notification, ct);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, CancellationToken ct)
    {
        return await context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public void Update(Notification notification)
    {
        context.Notifications.Update(notification);
    }
}
