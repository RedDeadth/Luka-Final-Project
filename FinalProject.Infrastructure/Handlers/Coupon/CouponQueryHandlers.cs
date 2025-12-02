using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CouponDtos;
using FinalProject.Application.Features.Coupon;
using FinalProject.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Coupon;

public class GetCouponByCodeQueryHandler : IRequestHandler<GetCouponByCodeQuery, Result<CouponResponseDto>>
{
    private readonly ICouponService _couponService;

    public GetCouponByCodeQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponResponseDto>> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var coupon = await _couponService.GetCouponByCodeAsync(request.Code);
            return Result<CouponResponseDto>.Ok(coupon);
        }
        catch (Exception ex)
        {
            return Result<CouponResponseDto>.Failure(ex.Message);
        }
    }
}

public class GetCouponsByCampaignQueryHandler : IRequestHandler<GetCouponsByCampaignQuery, Result<PaginatedResult<CouponResponseDto>>>
{
    private readonly ICouponService _couponService;

    public GetCouponsByCampaignQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<PaginatedResult<CouponResponseDto>>> Handle(GetCouponsByCampaignQuery request, CancellationToken cancellationToken)
    {
        var query = _couponService.GetCouponsByCampaign(request.CampaignId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<CouponResponseDto>>.Ok(new PaginatedResult<CouponResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}

public class GetCouponsBySupplierQueryHandler : IRequestHandler<GetCouponsBySupplierQuery, Result<PaginatedResult<CouponResponseDto>>>
{
    private readonly ICouponService _couponService;

    public GetCouponsBySupplierQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<PaginatedResult<CouponResponseDto>>> Handle(GetCouponsBySupplierQuery request, CancellationToken cancellationToken)
    {
        var query = _couponService.GetCouponsBySupplier(request.SupplierId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<CouponResponseDto>>.Ok(new PaginatedResult<CouponResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}

public class ValidateCouponQueryHandler : IRequestHandler<ValidateCouponQuery, Result<bool>>
{
    private readonly ICouponService _couponService;

    public ValidateCouponQueryHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<bool>> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var isValid = await _couponService.ValidateCouponAsync(request.Code);
        return Result<bool>.Ok(isValid);
    }
}
