using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinalProject.Infrastructure.Jobs;

/// <summary>
/// Job para limpiar datos antiguos del sistema
/// Se ejecuta semanalmente los domingos a las 02:00
/// </summary>
public class DataCleanupJob
{
    private readonly LukitasDbContext _context;
    private readonly ILogger<DataCleanupJob> _logger;

    public DataCleanupJob(LukitasDbContext context, ILogger<DataCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("[Hangfire] Iniciando DataCleanupJob...");

            var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6));
            int totalDeleted = 0;

            var oldExpiredCoupons = await _context.Coupons
                .Where(c => c.Active == false && c.ExpirationDate < cutoffDate)
                .ToListAsync();

            if (oldExpiredCoupons.Any())
            {
                _context.Coupons.RemoveRange(oldExpiredCoupons);
                totalDeleted += oldExpiredCoupons.Count;
                _logger.LogInformation($"[Hangfire] {oldExpiredCoupons.Count} cupones antiguos eliminados.");
            }


            await _context.SaveChangesAsync();
            _logger.LogInformation($"[Hangfire] DataCleanupJob completado. Total de registros eliminados: {totalDeleted}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Hangfire] Error en DataCleanupJob");
            throw;
        }
    }
}
