using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class SupplierManagementService : ISupplierManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public SupplierManagementService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            SupplierTypeId = dto.SupplierTypeId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Status = "active"
        };

        await _unitOfWork.Suppliers.AddAsync(supplier);
        await _unitOfWork.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id);
    }

    public async Task<SupplierResponseDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        if (supplier == null) throw new Exception("Supplier not found");

        var supplierType = await _unitOfWork.SupplierTypes.GetByIdAsync(supplier.SupplierTypeId ?? 0);
        var productsCount = await _unitOfWork.Products.CountAsync(p => p.SupplierId == id);

        return new SupplierResponseDto
        {
            Id = supplier.Id,
            SupplierTypeId = supplier.SupplierTypeId ?? 0,
            SupplierTypeName = supplierType?.Name ?? "",
            Name = supplier.Name,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Status = supplier.Status ?? "active",
            TotalProducts = productsCount
        };
    }

    public IQueryable<SupplierResponseDto> GetAllSuppliers()
    {
        return _context.Suppliers
            .Include(s => s.SupplierType)
            .Include(s => s.Products)
            .Where(s => s.Status == "active")
            .Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                SupplierTypeId = s.SupplierTypeId ?? 0,
                SupplierTypeName = s.SupplierType!.Name,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Status = s.Status ?? "active",
                TotalProducts = s.Products.Count
            });
    }

    public async Task<bool> UpdateSupplierAsync(int id, CreateSupplierDto dto)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        if (supplier == null) return false;

        supplier.SupplierTypeId = dto.SupplierTypeId;
        supplier.Name = dto.Name;
        supplier.Email = dto.Email;
        supplier.Phone = dto.Phone;

        _unitOfWork.Suppliers.Update(supplier);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        if (supplier == null) return false;

        supplier.Status = "inactive";
        _unitOfWork.Suppliers.Update(supplier);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
