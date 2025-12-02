using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.DTOs.StatisticsDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;
    private static decimal _lukasToUsdRate = 0.10m;

    public AdminService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public IQueryable<CompanyProfileDto> GetPendingCompanies()
    {
        return _context.Users
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
            });
    }

    public async Task<bool> ApproveCompanyAsync(CompanyApprovalDto dto)
    {
        var company = await _unitOfWork.Users.GetByIdAsync(dto.CompanyId);
        if (company == null) return false;

        company.Active = dto.Approved;
        _unitOfWork.Users.Update(company);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<SystemStatisticsDto> GetSystemStatisticsAsync()
    {
        var totalUsers = await _unitOfWork.Users.CountAsync();
        var totalStudents = await _unitOfWork.Users.CountAsync(u => u.RoleId == 1);
        var totalCompanies = await _unitOfWork.Users.CountAsync(u => u.RoleId == 2);
        var totalSuppliers = await _unitOfWork.Suppliers.CountAsync();
        var activeCampaigns = await _unitOfWork.Campaigns.CountAsync(c => c.Active == true);
        
        var accounts = await _unitOfWork.Accounts.GetAllAsync(1, 100);
        var totalLukasInCirculation = accounts.Items.Sum(a => a.Balance ?? 0);
        
        var sales = await _unitOfWork.Sales.GetAllAsync(1, 100);
        var totalLukasSpent = sales.Items.Sum(s => s.Total);
        var totalTransactions = await _unitOfWork.Sales.CountAsync();

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
        var company = await _unitOfWork.Users.GetByIdAsync(dto.CompanyId);
        if (company == null) return false;

        var companyAccount = await _unitOfWork.Accounts
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
            await _unitOfWork.Accounts.AddAsync(companyAccount);
            await _unitOfWork.SaveChangesAsync();
        }

        companyAccount.Balance += dto.Amount;
        _unitOfWork.Accounts.Update(companyAccount);
        await _unitOfWork.SaveChangesAsync();

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
