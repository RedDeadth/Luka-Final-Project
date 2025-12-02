using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly LukitasDbContext _context;

    public ProductService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            SupplierId = dto.SupplierId,
            ProductTypeId = dto.ProductTypeId,
            Code = dto.Code,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            Status = "active"
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) throw new Exception("Product not found");

        return new ProductResponseDto
        {
            Id = product.Id,
            SupplierId = product.SupplierId ?? 0,
            SupplierName = product.Supplier?.Name ?? "",
            ProductTypeId = product.ProductTypeId ?? 0,
            ProductTypeName = product.ProductType?.Name ?? "",
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock ?? 0,
            Status = product.Status ?? "active"
        };
    }

    public async Task<List<ProductResponseDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.ProductType)
            .Where(p => p.Status == "active")
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                SupplierId = p.SupplierId ?? 0,
                SupplierName = p.Supplier!.Name,
                ProductTypeId = p.ProductTypeId ?? 0,
                ProductTypeName = p.ProductType!.Name,
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock ?? 0,
                Status = p.Status ?? "active"
            })
            .ToListAsync();
    }

    public async Task<List<ProductResponseDto>> GetProductsBySupplierAsync(int supplierId)
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.ProductType)
            .Where(p => p.SupplierId == supplierId && p.Status == "active")
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                SupplierId = p.SupplierId ?? 0,
                SupplierName = p.Supplier!.Name,
                ProductTypeId = p.ProductTypeId ?? 0,
                ProductTypeName = p.ProductType!.Name,
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock ?? 0,
                Status = p.Status ?? "active"
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Status = dto.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.Status = "inactive";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        product.Stock = quantity;
        await _context.SaveChangesAsync();
        return true;
    }
}
