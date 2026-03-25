using Hangfire;
using FinalProject.Infrastructure.Jobs;

namespace FinalProject.API.Extensions;

/// <summary>
/// Configuración de Jobs recurrentes de Hangfire
/// </summary>
public static class HangfireJobsConfiguration
{
    public static void ConfigureRecurringJobs()
    {
        RecurringJob.AddOrUpdate<ExpireCouponsJob>(
            "expire-coupons-daily",
            job => job.ExecuteAsync(),
            Cron.Daily(0, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        RecurringJob.AddOrUpdate<DataCleanupJob>(
            "data-cleanup-weekly",
            job => job.ExecuteAsync(),
            Cron.Weekly(DayOfWeek.Sunday, 2, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        RecurringJob.AddOrUpdate<DailyStatisticsJob>(
            "daily-statistics",
            job => job.ExecuteAsync(),
            Cron.Daily(23, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}
