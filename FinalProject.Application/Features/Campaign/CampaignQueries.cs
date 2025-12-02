using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;

namespace FinalProject.Application.Features.Campaign;

public record GetActiveCampaignsQuery(int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<CampaignResponseDto>>>;
public record GetCampaignByIdQuery(int CampaignId) : IQuery<Result<CampaignResponseDto>>;
public record GetCompanyCampaignsQuery(int CompanyUserId, int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<CampaignResponseDto>>>;
