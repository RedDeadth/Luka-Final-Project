using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Interfaces;

public interface IStudentService
{
    IQueryable<CampaignResponseDto> GetAvailableCampaigns(int studentId);
    Task<decimal> GetLukasBalanceAsync(int studentId);
    Task<bool> PurchaseProductsAsync(ProductPurchaseDto dto);
}
