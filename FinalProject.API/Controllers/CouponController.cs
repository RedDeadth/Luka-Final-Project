using FinalProject.Application.DTOs.CouponDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
    {
        try
        {
            var coupon = await _couponService.CreateCouponAsync(dto);
            return Ok(new { success = true, data = coupon });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetCouponByCode(string code)
    {
        try
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            return Ok(new { success = true, data = coupon });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetCouponsByCampaign(int campaignId)
    {
        var coupons = await _couponService.GetCouponsByCampaign(campaignId).ToListAsync();
        return Ok(new { success = true, data = coupons });
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetCouponsBySupplier(int supplierId)
    {
        var coupons = await _couponService.GetCouponsBySupplier(supplierId).ToListAsync();
        return Ok(new { success = true, data = coupons });
    }

    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCoupon(string code)
    {
        var isValid = await _couponService.ValidateCouponAsync(code);
        return Ok(new { success = true, valid = isValid });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateCoupon(int id)
    {
        var result = await _couponService.DeactivateCouponAsync(id);
        if (result)
            return Ok(new { success = true, message = "Coupon deactivated successfully" });
        
        return NotFound(new { success = false, message = "Coupon not found" });
    }
}
