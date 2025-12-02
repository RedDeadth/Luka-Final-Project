using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Features.Coupon;
using FinalProject.Application.Features.Supplier;
using FinalProject.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Supplier;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<PaginatedResult<SupplierResponseDto>>>
{
    private readonly ISupplierManagementService _supplierService;

    public GetAllSuppliersQueryHandler(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<Result<PaginatedResult<SupplierResponseDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _supplierService.GetAllSuppliers();
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<SupplierResponseDto>>.Ok(new PaginatedResult<SupplierResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierResponseDto>>
{
    private readonly ISupplierManagementService _supplierService;

    public GetSupplierByIdQueryHandler(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<Result<SupplierResponseDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(request.Id);
            return Result<SupplierResponseDto>.Ok(supplier);
        }
        catch (Exception ex)
        {
            return Result<SupplierResponseDto>.Failure(ex.Message);
        }
    }
}
