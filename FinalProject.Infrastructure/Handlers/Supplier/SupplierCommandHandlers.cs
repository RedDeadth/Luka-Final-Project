using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Features.Supplier;
using FinalProject.Application.Interfaces;
using MediatR;

namespace FinalProject.Infrastructure.Handlers.Supplier;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierResponseDto>>
{
    private readonly ISupplierManagementService _supplierService;

    public CreateSupplierCommandHandler(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<Result<SupplierResponseDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var supplier = await _supplierService.CreateSupplierAsync(request.Dto);
            return Result<SupplierResponseDto>.Ok(supplier);
        }
        catch (Exception ex)
        {
            return Result<SupplierResponseDto>.Failure(ex.Message);
        }
    }
}

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<bool>>
{
    private readonly ISupplierManagementService _supplierService;

    public UpdateSupplierCommandHandler(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var result = await _supplierService.UpdateSupplierAsync(request.Id, request.Dto);
        return result 
            ? Result<bool>.Ok(true) 
            : Result<bool>.Failure("Supplier not found");
    }
}

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<bool>>
{
    private readonly ISupplierManagementService _supplierService;

    public DeleteSupplierCommandHandler(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<Result<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var result = await _supplierService.DeleteSupplierAsync(request.Id);
        return result 
            ? Result<bool>.Ok(true) 
            : Result<bool>.Failure("Supplier not found");
    }
}
