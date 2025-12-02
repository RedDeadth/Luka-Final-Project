using FinalProject.Application.Common;
using FinalProject.Application.DTOs.SupplierDtos;
using MediatR;

namespace FinalProject.Application.Features.Supplier;

// Commands
public record CreateSupplierCommand(CreateSupplierDto Dto) : IRequest<Result<SupplierResponseDto>>;
public record UpdateSupplierCommand(int Id, CreateSupplierDto Dto) : IRequest<Result<bool>>;
public record DeleteSupplierCommand(int Id) : IRequest<Result<bool>>;
