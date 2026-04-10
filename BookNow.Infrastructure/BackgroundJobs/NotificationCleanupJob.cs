using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookNow.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire recurring job that runs nightly at 2am UTC.
/// Deletes all notifications older than 90 days, and read notifications older than 30 days.
/// </summary>
public class NotificationCleanupJob(
    BookNowDbContext dbContext,
    ILogger<NotificationCleanupJob> logger)
{
    public const string JobId = "notification-cleanup-nightly";

    public async Task RunAsync()
    {
        var now = DateTime.UtcNow;
        var cutoff90Days = now.AddDays(-90);
        var cutoff30Days = now.AddDays(-30);

        logger.LogInformation(
            "NotificationCleanupJob starting. Cutoffs: 90-day={Cutoff90}, 30-day={Cutoff30}",
            cutoff90Days, cutoff30Days);

        // Step 1: Delete every notification older than 90 days (read or unread)
        var deletedAll = await dbContext.Notifications
            .Where(n => n.CreatedAt < cutoff90Days)
            .ExecuteDeleteAsync();

        // Step 2: Delete read notifications older than 30 days
        // (those < 90 days that are already read — the rest were caught by step 1)
        var deletedRead = await dbContext.Notifications
            .Where(n => n.IsRead && n.CreatedAt < cutoff30Days)
            .ExecuteDeleteAsync();

        logger.LogInformation(
            "NotificationCleanupJob completed. " +
            "Deleted {DeletedAll} notifications older than 90 days. " +
            "Deleted {DeletedRead} read notifications older than 30 days.",
            deletedAll, deletedRead);
    }
}
