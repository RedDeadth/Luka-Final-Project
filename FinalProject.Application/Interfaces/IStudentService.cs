using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Interfaces;

public interface IStudentService
{
    Task<List<CampaignResponseDto>> GetAvailableCampaignsAsync(int studentId);
    Task<decimal> GetLukasBalanceAsync(int studentId);
    Task<bool> PurchaseProductsAsync(ProductPurchaseDto dto);
}
