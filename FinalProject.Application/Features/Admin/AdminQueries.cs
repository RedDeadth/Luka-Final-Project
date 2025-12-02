using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.StatisticsDtos;

namespace FinalProject.Application.Features.Admin;

public record GetPendingCompaniesQuery(int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<CompanyProfileDto>>>;
public record GetSystemStatisticsQuery : IQuery<Result<SystemStatisticsDto>>;
