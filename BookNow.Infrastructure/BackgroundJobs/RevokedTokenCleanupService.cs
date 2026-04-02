using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookNow.Infrastructure.BackgroundJobs;

public class RevokedTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<RevokedTokenCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BookNowDbContext>();

                var deleted = await dbContext.RevokedTokens
                    .Where(t => t.ExpiresAt < DateTime.UtcNow)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                    logger.LogInformation("{Count} expired revoked tokens cleaned up.", deleted);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up revoked tokens.");
            }

            // Runs once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}