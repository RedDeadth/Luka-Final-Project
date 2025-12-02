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
    private const int MaxPageSize = 100;

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
    public async Task<IActionResult> GetCouponsByCampaign(int campaignId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _couponService.GetCouponsByCampaign(campaignId);
        var totalCount = await query.CountAsync();
        var coupons = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = coupons,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetCouponsBySupplier(int supplierId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _couponService.GetCouponsBySupplier(supplierId);
        var totalCount = await query.CountAsync();
        var coupons = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = coupons,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
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
