using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;

namespace FinalProject.Application.Features.Student;

public record GetAvailableCampaignsQuery(int StudentId, int Page = 1, int PageSize = 20) : IQuery<Result<PaginatedResponse<CampaignResponseDto>>>;
public record GetStudentBalanceQuery(int StudentId) : IQuery<Result<decimal>>;
