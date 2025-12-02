using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.StatisticsDtos;
using FinalProject.Application.Features.Admin;
using FinalProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Admin;

public class GetPendingCompaniesQueryHandler : IRequestHandler<GetPendingCompaniesQuery, Result<PaginatedResponse<CompanyProfileDto>>>
{
    private readonly LukitasDbContext _context;
    private const int MaxPageSize = 100;
    public GetPendingCompaniesQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<PaginatedResponse<CompanyProfileDto>>> Handle(GetPendingCompaniesQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);

        var query = _context.Users.Where(u => u.RoleId == 2 && u.Active == false);
        var totalCount = await query.CountAsync(cancellationToken);

        var companies = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new CompanyProfileDto
            {
                Id = u.Id, CompanyName = u.Company, Email = u.Email,
                ContactPerson = $"{u.FirstName} {u.LastName}", Phone = "", Approved = u.Active ?? false,
                LukasBalance = u.Accounts.Where(a => a.Status == "active").Sum(a => a.Balance ?? 0),
                ActiveCampaigns = u.Campaigns.Count(c => c.Active == true)
            }).ToListAsync(cancellationToken);

        return Result<PaginatedResponse<CompanyProfileDto>>.Ok(new PaginatedResponse<CompanyProfileDto>
        {
            Data = companies,
            Pagination = new PaginationInfo { Page = page, PageSize = pageSize, TotalCount = totalCount, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) }
        });
    }
}

public class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, Result<SystemStatisticsDto>>
{
    private readonly LukitasDbContext _context;
    public GetSystemStatisticsQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<SystemStatisticsDto>> Handle(GetSystemStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stats = new SystemStatisticsDto
        {
            TotalUsers = await _context.Users.CountAsync(cancellationToken),
            TotalStudents = await _context.Users.CountAsync(u => u.RoleId == 1, cancellationToken),
            TotalCompanies = await _context.Users.CountAsync(u => u.RoleId == 2, cancellationToken),
            TotalSuppliers = await _context.Suppliers.CountAsync(cancellationToken),
            ActiveCampaigns = await _context.Campaigns.CountAsync(c => c.Active == true, cancellationToken),
            TotalLukasInCirculation = await _context.Accounts.SumAsync(a => a.Balance ?? 0, cancellationToken),
            TotalLukasSpent = await _context.Sales.SumAsync(s => s.Total, cancellationToken),
            TotalTransactions = await _context.Sales.CountAsync(cancellationToken),
            RecentActivity = await _context.Sales.OrderByDescending(s => s.SaleDate).Take(10)
                .Select(s => new ActivityLogDto { Timestamp = s.SaleDate ?? DateTime.Now, ActivityType = "Purchase", Description = $"Purchase of {s.Total} Lukas", UserEmail = s.Account!.User!.Email })
                .ToListAsync(cancellationToken)
        };
        return Result<SystemStatisticsDto>.Ok(stats);
    }
}

public class ApproveCompanyCommandHandler : IRequestHandler<ApproveCompanyCommand, Result<bool>>
{
    private readonly LukitasDbContext _context;
    public ApproveCompanyCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(ApproveCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Users.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company == null) return Result<bool>.NotFound("Company not found");

        company.Active = request.Approved;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class EmitLukasCommandHandler : IRequestHandler<EmitLukasCommand, Result<bool>>
{
    private readonly LukitasDbContext _context;
    public EmitLukasCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(EmitLukasCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Users.FindAsync(new object[] { request.CompanyId }, cancellationToken);
        if (company == null) return Result<bool>.NotFound("Company not found");

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.CompanyId && a.Status == "active", cancellationToken);
        if (account == null)
        {
            account = new Account { UserId = request.CompanyId, AccountNumber = $"ACC-COMPANY-{request.CompanyId}-{DateTime.Now.Ticks}", Balance = 0, Status = "active" };
            _context.Accounts.Add(account);
        }
        account.Balance += request.Amount;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}
