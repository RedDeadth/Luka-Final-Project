using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.DTOs.StatisticsDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly LukitasDbContext _context;
    private static decimal _lukasToUsdRate = 0.10m;

    public AdminService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyProfileDto>> GetPendingCompaniesAsync()
    {
        var companies = await _context.Users
            .Where(u => u.RoleId == 2 && u.Active == false)
            .Select(u => new CompanyProfileDto
            {
                Id = u.Id,
                CompanyName = u.Company,
                Email = u.Email,
                ContactPerson = $"{u.FirstName} {u.LastName}",
                Phone = "",
                Approved = u.Active ?? false,
                LukasBalance = u.Accounts.Where(a => a.Status == "active").Sum(a => a.Balance ?? 0),
                ActiveCampaigns = u.Campaigns.Count(c => c.Active == true)
            })
            .ToListAsync();

        return companies;
    }

    public async Task<bool> ApproveCompanyAsync(CompanyApprovalDto dto)
    {
        var company = await _context.Users.FindAsync(dto.CompanyId);
        if (company == null) return false;

        company.Active = dto.Approved;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<SystemStatisticsDto> GetSystemStatisticsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalStudents = await _context.Users.CountAsync(u => u.RoleId == 1);
        var totalCompanies = await _context.Users.CountAsync(u => u.RoleId == 2);
        var totalSuppliers = await _context.Suppliers.CountAsync();
        var activeCampaigns = await _context.Campaigns.CountAsync(c => c.Active == true);
        var totalLukasInCirculation = await _context.Accounts.SumAsync(a => a.Balance ?? 0);
        var totalLukasSpent = await _context.Sales.SumAsync(s => s.Total);
        var totalTransactions = await _context.Sales.CountAsync();

        var recentActivity = await _context.Sales
            .OrderByDescending(s => s.SaleDate)
            .Take(10)
            .Include(s => s.Account)
            .ThenInclude(a => a!.User)
            .Select(s => new ActivityLogDto
            {
                Timestamp = s.SaleDate ?? DateTime.Now,
                ActivityType = "Purchase",
                Description = $"Purchase of {s.Total} Lukas",
                UserEmail = s.Account!.User!.Email
            })
            .ToListAsync();

        return new SystemStatisticsDto
        {
            TotalUsers = totalUsers,
            TotalStudents = totalStudents,
            TotalCompanies = totalCompanies,
            TotalSuppliers = totalSuppliers,
            ActiveCampaigns = activeCampaigns,
            TotalLukasInCirculation = totalLukasInCirculation,
            TotalLukasSpent = totalLukasSpent,
            TotalTransactions = totalTransactions,
            RecentActivity = recentActivity
        };
    }

    public async Task<bool> EmitLukasAsync(EmitLukasDto dto)
    {
        var company = await _context.Users.FindAsync(dto.CompanyId);
        if (company == null) return false;

        var companyAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == dto.CompanyId && a.Status == "active");

        if (companyAccount == null)
        {
            companyAccount = new Account
            {
                UserId = dto.CompanyId,
                AccountNumber = $"ACC-COMPANY-{dto.CompanyId}-{DateTime.Now.Ticks}",
                Balance = 0,
                Status = "active"
            };
            _context.Accounts.Add(companyAccount);
            await _context.SaveChangesAsync();
        }

        companyAccount.Balance += dto.Amount;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<LukasValueDto> GetLukasValueAsync()
    {
        return await Task.FromResult(new LukasValueDto
        {
            LukasToUsdRate = _lukasToUsdRate,
            UsdToLukasRate = 1 / _lukasToUsdRate,
            LastUpdated = DateTime.Now
        });
    }

    public async Task<bool> UpdateLukasValueAsync(UpdateLukasValueDto dto)
    {
        _lukasToUsdRate = dto.LukasToUsdRate;
        return await Task.FromResult(true);
    }
}
