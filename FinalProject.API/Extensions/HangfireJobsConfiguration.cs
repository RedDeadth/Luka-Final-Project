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
        // ExpireCouponsJob - Ejecutar diariamente a las 00:00 UTC
        RecurringJob.AddOrUpdate<ExpireCouponsJob>(
            "expire-coupons-daily",
            job => job.ExecuteAsync(),
            Cron.Daily(0, 0), // 00:00 UTC
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // DataCleanupJob - Ejecutar semanalmente los domingos a las 02:00 UTC
        RecurringJob.AddOrUpdate<DataCleanupJob>(
            "data-cleanup-weekly",
            job => job.ExecuteAsync(),
            Cron.Weekly(DayOfWeek.Sunday, 2, 0), // Domingo 02:00 UTC
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // DailyStatisticsJob - Ejecutar diariamente a las 23:00 UTC
        RecurringJob.AddOrUpdate<DailyStatisticsJob>(
            "daily-statistics",
            job => job.ExecuteAsync(),
            Cron.Daily(23, 0), // 23:00 UTC
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}
