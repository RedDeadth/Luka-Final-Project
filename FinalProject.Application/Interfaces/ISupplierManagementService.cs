using FinalProject.Application.DTOs.SupplierDtos;

namespace FinalProject.Application.Interfaces;

public interface ISupplierManagementService
{
    Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto);
    Task<SupplierResponseDto> GetSupplierByIdAsync(int id);
    Task<List<SupplierResponseDto>> GetAllSuppliersAsync();
    Task<bool> UpdateSupplierAsync(int id, CreateSupplierDto dto);
    Task<bool> DeleteSupplierAsync(int id);
}
