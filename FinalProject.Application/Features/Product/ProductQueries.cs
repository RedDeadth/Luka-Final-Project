using FinalProject.Application.Common;
using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Features.Product;

public record GetAllProductsQuery(int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<ProductResponseDto>>>;
public record GetProductByIdQuery(int ProductId) : IQuery<Result<ProductResponseDto>>;
public record GetProductsBySupplierQuery(int SupplierId, int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<ProductResponseDto>>>;
