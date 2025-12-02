using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;

namespace FinalProject.Application.Features.Campaign;

public record CreateCampaignCommand(
    int CompanyUserId,
    string Name,
    string? Description,
    string CampaignType,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Schedule,
    string? Location,
    string? ContactNumber,
    List<string>? ImageUrls
) : ICommand<Result<CampaignResponseDto>>;

public record EnrollStudentCommand(int CampaignId, int StudentId) : ICommand<Result<bool>>;
