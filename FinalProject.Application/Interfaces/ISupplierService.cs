using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Interfaces;

public interface ISupplierService
{
    Task<decimal> GetLukasBalanceAsync(int supplierId);
    Task<bool> ConvertLukasToRealMoneyAsync(int supplierId, decimal amount);
}
