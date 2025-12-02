using FinalProject.Application.DTOs.CouponDtos;

namespace FinalProject.Application.Interfaces;

public interface ICouponService
{
    Task<CouponResponseDto> CreateCouponAsync(CreateCouponDto dto);
    Task<CouponResponseDto> GetCouponByCodeAsync(string code);
    IQueryable<CouponResponseDto> GetCouponsByCampaign(int campaignId);
    IQueryable<CouponResponseDto> GetCouponsBySupplier(int supplierId);
    Task<bool> ValidateCouponAsync(string code);
    Task<bool> DeactivateCouponAsync(int id);
}
