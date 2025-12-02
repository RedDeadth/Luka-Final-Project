using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinalProject.Infrastructure.Jobs;

/// <summary>
/// Job para expirar cupones vencidos automáticamente
/// Se ejecuta diariamente a las 00:00
/// </summary>
public class ExpireCouponsJob
{
    private readonly LukitasDbContext _context;
    private readonly ILogger<ExpireCouponsJob> _logger;

    public ExpireCouponsJob(LukitasDbContext context, ILogger<ExpireCouponsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("[Hangfire] Iniciando ExpireCouponsJob...");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredCoupons = await _context.Coupons
                .Where(c => c.ExpirationDate < today && c.Active == true)
                .ToListAsync();

            if (expiredCoupons.Any())
            {
                foreach (var coupon in expiredCoupons)
                {
                    coupon.Active = false;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"[Hangfire] ExpireCouponsJob completado. {expiredCoupons.Count} cupones expirados.");
            }
            else
            {
                _logger.LogInformation("[Hangfire] ExpireCouponsJob completado. No hay cupones para expirar.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Hangfire] Error en ExpireCouponsJob");
            throw;
        }
    }
}
