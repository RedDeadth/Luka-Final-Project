using FinalProject.Application.Common;
using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Features.Student;

public record PurchaseProductsCommand(
    int StudentId,
    int SupplierId,
    List<ProductItemDto> Items
) : ICommand<Result<bool>>;
