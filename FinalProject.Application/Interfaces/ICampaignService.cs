using FinalProject.Application.DTOs.CampaignDtos;

namespace FinalProject.Application.Interfaces;

public interface ICampaignService
{
    Task<CampaignResponseDto> CreateCampaignAsync(int companyUserId, CreateCampaignDto dto);
    IQueryable<CampaignResponseDto> GetActiveCampaigns();
    Task<CampaignResponseDto> GetCampaignByIdAsync(int campaignId);
    Task<bool> EnrollStudentAsync(EnrollCampaignDto dto);
    IQueryable<CampaignResponseDto> GetCompanyCampaigns(int companyUserId);
}
