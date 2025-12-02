using FinalProject.Application.DTOs.SupplierDtos;

namespace FinalProject.Application.Interfaces;

public interface ISupplierManagementService
{
    Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto);
    Task<SupplierResponseDto> GetSupplierByIdAsync(int id);
    IQueryable<SupplierResponseDto> GetAllSuppliers();
    Task<bool> UpdateSupplierAsync(int id, CreateSupplierDto dto);
    Task<bool> DeleteSupplierAsync(int id);
}
