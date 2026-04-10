using Hangfire;
using Hangfire.PostgreSql;
using BookNow.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookNow.Presentation.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = DbExtensions.GetConnectionString(configuration);

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseFilter(new AutomaticRetryAttribute { Attempts = 3 })
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            }));

        services.AddHangfireServer();

        return services;
    }

    public static IEndpointRouteBuilder MapHangfireDashboardCustom(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter() },
            DashboardTitle = "BookNow Background Jobs"
        });

        return endpoints;
    }

    /// <summary>
    /// Registers all recurring Hangfire jobs.
    /// Call this after app.MapHangfireDashboardCustom() in Program.cs.
    /// </summary>
    public static WebApplication UseHangfireRecurringJobs(this WebApplication app)
    {
        // Nightly at 2:00 AM UTC — clean up stale notifications
        RecurringJob.AddOrUpdate<NotificationCleanupJob>(
            recurringJobId: NotificationCleanupJob.JobId,
            methodCall: job => job.RunAsync(),
            cronExpression: "0 2 * * *",   // cron: minute=0, hour=2, every day
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        return app;
    }
}
