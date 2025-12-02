using FinalProject.Application.Common;
using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Features.Product;

public record CreateProductCommand(
    int SupplierId,
    int ProductTypeId,
    string Code,
    string Name,
    decimal Price,
    int Stock
) : ICommand<Result<ProductResponseDto>>;
