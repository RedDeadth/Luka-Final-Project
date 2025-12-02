using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class SupplierManagementService : ISupplierManagementService
{
    private readonly LukitasDbContext _context;

    public SupplierManagementService(LukitasDbContext context)
    {
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

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id);
    }

    public async Task<SupplierResponseDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.SupplierType)
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) throw new Exception("Supplier not found");

        return new SupplierResponseDto
        {
            Id = supplier.Id,
            SupplierTypeId = supplier.SupplierTypeId ?? 0,
            SupplierTypeName = supplier.SupplierType?.Name ?? "",
            Name = supplier.Name,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Status = supplier.Status ?? "active",
            TotalProducts = supplier.Products.Count
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
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return false;

        supplier.SupplierTypeId = dto.SupplierTypeId;
        supplier.Name = dto.Name;
        supplier.Email = dto.Email;
        supplier.Phone = dto.Phone;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return false;

        supplier.Status = "inactive";
        await _context.SaveChangesAsync();
        return true;
    }
}
