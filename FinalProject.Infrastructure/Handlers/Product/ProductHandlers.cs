using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Features.Product;
using FinalProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Product;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedResponse<ProductResponseDto>>>
{
    private readonly LukitasDbContext _context;
    private const int MaxPageSize = 100;
    public GetAllProductsQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<PaginatedResponse<ProductResponseDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);

        var query = _context.Products.Where(p => p.Status == "active");
        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id, SupplierId = p.SupplierId ?? 0, SupplierName = p.Supplier!.Name,
                ProductTypeId = p.ProductTypeId ?? 0, ProductTypeName = p.ProductType!.Name,
                Code = p.Code, Name = p.Name, Price = p.Price, Stock = p.Stock ?? 0, Status = p.Status ?? "active"
            }).ToListAsync(cancellationToken);

        return Result<PaginatedResponse<ProductResponseDto>>.Ok(new PaginatedResponse<ProductResponseDto>
        {
            Data = products,
            Pagination = new PaginationInfo { Page = page, PageSize = pageSize, TotalCount = totalCount, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) }
        });
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponseDto>>
{
    private readonly LukitasDbContext _context;
    public CreateProductCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Domain.Entities.Product
        {
            SupplierId = request.SupplierId, ProductTypeId = request.ProductTypeId,
            Code = request.Code, Name = request.Name, Price = request.Price, Stock = request.Stock, Status = "active"
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Products.Include(p => p.Supplier).Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        return Result<ProductResponseDto>.Created(new ProductResponseDto
        {
            Id = created!.Id, SupplierId = created.SupplierId ?? 0, SupplierName = created.Supplier?.Name ?? "",
            ProductTypeId = created.ProductTypeId ?? 0, ProductTypeName = created.ProductType?.Name ?? "",
            Code = created.Code, Name = created.Name, Price = created.Price, Stock = created.Stock ?? 0, Status = created.Status ?? "active"
        });
    }
}
