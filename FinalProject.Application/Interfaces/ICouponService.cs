using FinalProject.Application.DTOs.CouponDtos;

namespace FinalProject.Application.Interfaces;

public interface ICouponService
{
    Task<CouponResponseDto> CreateCouponAsync(CreateCouponDto dto);
    Task<CouponResponseDto> GetCouponByCodeAsync(string code);
    Task<List<CouponResponseDto>> GetCouponsByCampaignAsync(int campaignId);
    Task<List<CouponResponseDto>> GetCouponsBySupplierAsync(int supplierId);
    Task<bool> ValidateCouponAsync(string code);
    Task<bool> DeactivateCouponAsync(int id);
}
