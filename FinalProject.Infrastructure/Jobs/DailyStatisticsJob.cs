using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinalProject.Infrastructure.Jobs;

/// <summary>
/// Job para generar estadísticas diarias del sistema
/// Se ejecuta diariamente a las 23:00
/// </summary>
public class DailyStatisticsJob
{
    private readonly LukitasDbContext _context;
    private readonly ILogger<DailyStatisticsJob> _logger;

    public DailyStatisticsJob(LukitasDbContext context, ILogger<DailyStatisticsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("[Hangfire] Iniciando DailyStatisticsJob...");

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var todayDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);

            // Calcular estadísticas del día
            var todaySales = await _context.Sales
                .Where(s => s.SaleDate >= today && s.SaleDate < tomorrow)
                .CountAsync();

            var todayTransfers = await _context.Transfers
                .Where(t => t.TransferDate == todayDateOnly)
                .CountAsync();

            var totalLukasInSystem = await _context.Accounts
                .SumAsync(a => a.Balance ?? 0);

            var activeCampaigns = await _context.Campaigns
                .Where(c => c.Active == true)
                .CountAsync();

            var totalUsers = await _context.Users.CountAsync();

            _logger.LogInformation($"[Hangfire] Estadísticas del día:");
            _logger.LogInformation($"  - Ventas: {todaySales}");
            _logger.LogInformation($"  - Transferencias: {todayTransfers}");
            _logger.LogInformation($"  - Total Lukitas en sistema: {totalLukasInSystem}");
            _logger.LogInformation($"  - Campañas activas: {activeCampaigns}");
            _logger.LogInformation($"  - Total usuarios: {totalUsers}");

            // Aquí podrías guardar estas estadísticas en una tabla de auditoría si lo deseas
            // Por ahora solo las registramos en los logs

            _logger.LogInformation("[Hangfire] DailyStatisticsJob completado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Hangfire] Error en DailyStatisticsJob");
            throw;
        }
    }
}
