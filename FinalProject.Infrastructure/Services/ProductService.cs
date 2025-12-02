using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public ProductService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) throw new Exception("Product not found");

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(product.SupplierId ?? 0);
        var productType = await _unitOfWork.ProductTypes.GetByIdAsync(product.ProductTypeId ?? 0);

        return new ProductResponseDto
        {
            Id = product.Id,
            SupplierId = product.SupplierId ?? 0,
            SupplierName = supplier?.Name ?? "",
            ProductTypeId = product.ProductTypeId ?? 0,
            ProductTypeName = productType?.Name ?? "",
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock ?? 0,
            Status = product.Status ?? "active"
        };
    }

    public IQueryable<ProductResponseDto> GetAllProducts()
    {
        return _context.Products
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
            });
    }

    public IQueryable<ProductResponseDto> GetProductsBySupplier(int supplierId)
    {
        return _context.Products
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
            });
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Status = dto.Status;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return false;

        product.Status = "inactive";
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return false;

        product.Stock = quantity;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
