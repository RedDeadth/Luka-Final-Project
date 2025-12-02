using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductResponseDto> GetProductByIdAsync(int id);
    IQueryable<ProductResponseDto> GetAllProducts();
    IQueryable<ProductResponseDto> GetProductsBySupplier(int supplierId);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
