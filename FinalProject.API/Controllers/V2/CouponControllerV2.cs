using FinalProject.Application.DTOs.CouponDtos;
using FinalProject.Application.Features.Coupon;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/[controller]")]
public class CouponControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;
    private const int MaxPageSize = 100;

    public CouponControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
    {
        var result = await _mediator.Send(new CreateCouponCommand(dto));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetCouponByCode(string code)
    {
        var result = await _mediator.Send(new GetCouponByCodeQuery(code));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetCouponsByCampaign(int campaignId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);

        var result = await _mediator.Send(new GetCouponsByCampaignQuery(campaignId, page, pageSize));
        return Ok(new
        {
            success = true,
            data = result.Data!.Data,
            pagination = new
            {
                page = result.Data.Page,
                pageSize = result.Data.PageSize,
                totalCount = result.Data.TotalCount,
                totalPages = result.Data.TotalPages
            }
        });
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetCouponsBySupplier(int supplierId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);

        var result = await _mediator.Send(new GetCouponsBySupplierQuery(supplierId, page, pageSize));
        return Ok(new
        {
            success = true,
            data = result.Data!.Data,
            pagination = new
            {
                page = result.Data.Page,
                pageSize = result.Data.PageSize,
                totalCount = result.Data.TotalCount,
                totalPages = result.Data.TotalPages
            }
        });
    }

    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCoupon(string code)
    {
        var result = await _mediator.Send(new ValidateCouponQuery(code));
        return Ok(new { success = true, valid = result.Data });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateCoupon(int id)
    {
        var result = await _mediator.Send(new DeactivateCouponCommand(id));
        return result.IsSuccess
            ? Ok(new { success = true, message = "Coupon deactivated successfully" })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }
}
