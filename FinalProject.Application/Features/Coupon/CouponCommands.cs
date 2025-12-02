using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CouponDtos;
using MediatR;

namespace FinalProject.Application.Features.Coupon;

// Commands
public record CreateCouponCommand(CreateCouponDto Dto) : IRequest<Result<CouponResponseDto>>;
public record DeactivateCouponCommand(int Id) : IRequest<Result<bool>>;
