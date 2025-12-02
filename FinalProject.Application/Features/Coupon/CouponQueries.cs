using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CouponDtos;
using MediatR;

namespace FinalProject.Application.Features.Coupon;

// Queries
public record GetCouponByCodeQuery(string Code) : IRequest<Result<CouponResponseDto>>;
public record GetCouponsByCampaignQuery(int CampaignId, int Page, int PageSize) : IRequest<Result<PaginatedResult<CouponResponseDto>>>;
public record GetCouponsBySupplierQuery(int SupplierId, int Page, int PageSize) : IRequest<Result<PaginatedResult<CouponResponseDto>>>;
public record ValidateCouponQuery(string Code) : IRequest<Result<bool>>;

// Resultado paginado
public class PaginatedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
