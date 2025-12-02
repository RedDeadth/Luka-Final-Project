using FinalProject.Application.DTOs.CampaignDtos;

namespace FinalProject.Application.Interfaces;

public interface ICampaignService
{
    Task<CampaignResponseDto> CreateCampaignAsync(int companyUserId, CreateCampaignDto dto);
    Task<List<CampaignResponseDto>> GetActiveCampaignsAsync();
    Task<CampaignResponseDto> GetCampaignByIdAsync(int campaignId);
    Task<bool> EnrollStudentAsync(EnrollCampaignDto dto);
    Task<List<CampaignResponseDto>> GetCompanyCampaignsAsync(int companyUserId);
}
