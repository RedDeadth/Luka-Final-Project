using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Features.Campaign;
using FinalProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Campaign;

public class GetActiveCampaignsQueryHandler : IRequestHandler<GetActiveCampaignsQuery, Result<PaginatedResponse<CampaignResponseDto>>>
{
    private readonly LukitasDbContext _context;
    private const int MaxPageSize = 100;

    public GetActiveCampaignsQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<PaginatedResponse<CampaignResponseDto>>> Handle(GetActiveCampaignsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);

        var query = _context.Campaigns.Where(c => c.Active == true && c.EndDate >= DateOnly.FromDateTime(DateTime.Now));
        var totalCount = await query.CountAsync(cancellationToken);

        var campaigns = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id, Name = c.Name, Description = c.Description, CampaignType = "General",
                Budget = c.Budget ?? 0, RemainingBudget = c.Budget ?? 0, StartDate = c.StartDate,
                EndDate = c.EndDate, Schedule = c.Schedule, Location = c.Location,
                ContactNumber = c.ContactNumber, Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count, CompanyName = c.User!.Company
            }).ToListAsync(cancellationToken);

        return Result<PaginatedResponse<CampaignResponseDto>>.Ok(new PaginatedResponse<CampaignResponseDto>
        {
            Data = campaigns,
            Pagination = new PaginationInfo { Page = page, PageSize = pageSize, TotalCount = totalCount, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) }
        });
    }
}

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<CampaignResponseDto>>
{
    private readonly LukitasDbContext _context;
    public GetCampaignByIdQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<CampaignResponseDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        var campaign = await _context.Campaigns.Include(c => c.User).Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == request.CampaignId, cancellationToken);

        if (campaign == null) return Result<CampaignResponseDto>.NotFound("Campaign not found");

        return Result<CampaignResponseDto>.Ok(new CampaignResponseDto
        {
            Id = campaign.Id, Name = campaign.Name, Description = campaign.Description, CampaignType = "General",
            Budget = campaign.Budget ?? 0, RemainingBudget = campaign.Budget ?? 0, StartDate = campaign.StartDate,
            EndDate = campaign.EndDate, Schedule = campaign.Schedule, Location = campaign.Location,
            ContactNumber = campaign.ContactNumber, Active = campaign.Active ?? true,
            EnrolledStudents = campaign.Accounts.Count, CompanyName = campaign.User!.Company
        });
    }
}

public class GetCompanyCampaignsQueryHandler : IRequestHandler<GetCompanyCampaignsQuery, Result<PaginatedResponse<CampaignResponseDto>>>
{
    private readonly LukitasDbContext _context;
    private const int MaxPageSize = 100;
    public GetCompanyCampaignsQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<PaginatedResponse<CampaignResponseDto>>> Handle(GetCompanyCampaignsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);

        var query = _context.Campaigns.Where(c => c.UserId == request.CompanyUserId);
        var totalCount = await query.CountAsync(cancellationToken);

        var campaigns = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id, Name = c.Name, Description = c.Description, CampaignType = "General",
                Budget = c.Budget ?? 0, RemainingBudget = c.Budget ?? 0, StartDate = c.StartDate,
                EndDate = c.EndDate, Schedule = c.Schedule, Location = c.Location,
                ContactNumber = c.ContactNumber, Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count, CompanyName = c.User!.Company
            }).ToListAsync(cancellationToken);

        return Result<PaginatedResponse<CampaignResponseDto>>.Ok(new PaginatedResponse<CampaignResponseDto>
        {
            Data = campaigns,
            Pagination = new PaginationInfo { Page = page, PageSize = pageSize, TotalCount = totalCount, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) }
        });
    }
}
