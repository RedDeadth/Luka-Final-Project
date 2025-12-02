using FinalProject.Application.Common;
using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Features.Coupon;
using MediatR;

namespace FinalProject.Application.Features.Supplier;

// Queries
public record GetAllSuppliersQuery(int Page, int PageSize) : IRequest<Result<PaginatedResult<SupplierResponseDto>>>;
public record GetSupplierByIdQuery(int Id) : IRequest<Result<SupplierResponseDto>>;
