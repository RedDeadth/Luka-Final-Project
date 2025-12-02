using FinalProject.Application.DTOs.CouponDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly LukitasDbContext _context;

    public CouponService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<CouponResponseDto> CreateCouponAsync(CreateCouponDto dto)
    {
        var coupon = new Coupon
        {
            CampaignId = dto.CampaignId,
            SupplierId = dto.SupplierId,
            Code = dto.Code,
            DiscountType = dto.DiscountType,
            DiscountValue = dto.DiscountValue,
            ExpirationDate = dto.ExpirationDate,
            Active = true
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return await GetCouponByCodeAsync(coupon.Code);
    }

    public async Task<CouponResponseDto> GetCouponByCodeAsync(string code)
    {
        var coupon = await _context.Coupons
            .Include(c => c.Campaign)
            .Include(c => c.Supplier)
            .FirstOrDefaultAsync(c => c.Code == code);

        if (coupon == null) throw new Exception("Coupon not found");

        return new CouponResponseDto
        {
            Id = coupon.Id,
            CampaignId = coupon.CampaignId ?? 0,
            CampaignName = coupon.Campaign?.Name ?? "",
            SupplierId = coupon.SupplierId ?? 0,
            SupplierName = coupon.Supplier?.Name ?? "",
            Code = coupon.Code,
            DiscountType = coupon.DiscountType ?? "",
            DiscountValue = coupon.DiscountValue,
            ExpirationDate = coupon.ExpirationDate,
            Active = coupon.Active ?? false
        };
    }

    public async Task<List<CouponResponseDto>> GetCouponsByCampaignAsync(int campaignId)
    {
        return await _context.Coupons
            .Include(c => c.Campaign)
            .Include(c => c.Supplier)
            .Where(c => c.CampaignId == campaignId && c.Active == true)
            .Select(c => new CouponResponseDto
            {
                Id = c.Id,
                CampaignId = c.CampaignId ?? 0,
                CampaignName = c.Campaign!.Name,
                SupplierId = c.SupplierId ?? 0,
                SupplierName = c.Supplier!.Name,
                Code = c.Code,
                DiscountType = c.DiscountType ?? "",
                DiscountValue = c.DiscountValue,
                ExpirationDate = c.ExpirationDate,
                Active = c.Active ?? false
            })
            .ToListAsync();
    }

    public async Task<List<CouponResponseDto>> GetCouponsBySupplierAsync(int supplierId)
    {
        return await _context.Coupons
            .Include(c => c.Campaign)
            .Include(c => c.Supplier)
            .Where(c => c.SupplierId == supplierId && c.Active == true)
            .Select(c => new CouponResponseDto
            {
                Id = c.Id,
                CampaignId = c.CampaignId ?? 0,
                CampaignName = c.Campaign!.Name,
                SupplierId = c.SupplierId ?? 0,
                SupplierName = c.Supplier!.Name,
                Code = c.Code,
                DiscountType = c.DiscountType ?? "",
                DiscountValue = c.DiscountValue,
                ExpirationDate = c.ExpirationDate,
                Active = c.Active ?? false
            })
            .ToListAsync();
    }

    public async Task<bool> ValidateCouponAsync(string code)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == code && c.Active == true);

        if (coupon == null) return false;

        return coupon.ExpirationDate >= DateOnly.FromDateTime(DateTime.Now);
    }

    public async Task<bool> DeactivateCouponAsync(int id)
    {
        var coupon = await _context.Coupons.FindAsync(id);
        if (coupon == null) return false;

        coupon.Active = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
