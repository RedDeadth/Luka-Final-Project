using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CouponDtos;
using FinalProject.Application.Features.Coupon;
using FinalProject.Application.Interfaces;
using MediatR;

namespace FinalProject.Infrastructure.Handlers.Coupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<CouponResponseDto>>
{
    private readonly ICouponService _couponService;

    public CreateCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<CouponResponseDto>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var coupon = await _couponService.CreateCouponAsync(request.Dto);
            return Result<CouponResponseDto>.Ok(coupon);
        }
        catch (Exception ex)
        {
            return Result<CouponResponseDto>.Failure(ex.Message);
        }
    }
}

public class DeactivateCouponCommandHandler : IRequestHandler<DeactivateCouponCommand, Result<bool>>
{
    private readonly ICouponService _couponService;

    public DeactivateCouponCommandHandler(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<Result<bool>> Handle(DeactivateCouponCommand request, CancellationToken cancellationToken)
    {
        var result = await _couponService.DeactivateCouponAsync(request.Id);
        return result 
            ? Result<bool>.Ok(true) 
            : Result<bool>.Failure("Coupon not found");
    }
}
