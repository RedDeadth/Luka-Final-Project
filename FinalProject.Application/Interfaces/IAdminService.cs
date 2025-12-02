using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.DTOs.StatisticsDtos;

namespace FinalProject.Application.Interfaces;

public interface IAdminService
{
    IQueryable<CompanyProfileDto> GetPendingCompanies();
    Task<bool> ApproveCompanyAsync(CompanyApprovalDto dto);
    Task<SystemStatisticsDto> GetSystemStatisticsAsync();
    Task<bool> EmitLukasAsync(EmitLukasDto dto);
    Task<LukasValueDto> GetLukasValueAsync();
    Task<bool> UpdateLukasValueAsync(UpdateLukasValueDto dto);
}
