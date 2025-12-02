using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class CampaignService : ICampaignService
{
    private readonly LukitasDbContext _context;

    public CampaignService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<CampaignResponseDto> CreateCampaignAsync(int companyUserId, CreateCampaignDto dto)
    {
        var company = await _context.Users.FindAsync(companyUserId);
        if (company == null) throw new Exception("Company not found");

        var campaign = new Campaign
        {
            UserId = companyUserId,
            Name = dto.Name,
            Description = $"{dto.Description}\nTipo: {dto.CampaignType}\nImágenes: {string.Join(", ", dto.ImageUrls ?? new List<string>())}",
            Budget = dto.Budget,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Schedule = dto.Schedule,
            Location = dto.Location,
            ContactNumber = dto.ContactNumber,
            Active = true
        };

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();

        return await GetCampaignByIdAsync(campaign.Id);
    }

    public async Task<List<CampaignResponseDto>> GetActiveCampaignsAsync()
    {
        return await _context.Campaigns
            .Where(c => c.Active == true && c.EndDate >= DateOnly.FromDateTime(DateTime.Now))
            .Include(c => c.User)
            .Include(c => c.Accounts)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CampaignType = "General",
                Budget = c.Budget ?? 0,
                RemainingBudget = c.Budget ?? 0,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Schedule = c.Schedule,
                Location = c.Location,
                ContactNumber = c.ContactNumber,
                Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count,
                CompanyName = c.User!.Company
            })
            .ToListAsync();
    }

    public async Task<CampaignResponseDto> GetCampaignByIdAsync(int campaignId)
    {
        var campaign = await _context.Campaigns
            .Include(c => c.User)
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == campaignId);

        if (campaign == null) throw new Exception("Campaign not found");

        return new CampaignResponseDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            CampaignType = "General",
            Budget = campaign.Budget ?? 0,
            RemainingBudget = campaign.Budget ?? 0,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            Schedule = campaign.Schedule,
            Location = campaign.Location,
            ContactNumber = campaign.ContactNumber,
            Active = campaign.Active ?? true,
            EnrolledStudents = campaign.Accounts.Count,
            CompanyName = campaign.User!.Company
        };
    }

    public async Task<bool> EnrollStudentAsync(EnrollCampaignDto dto)
    {
        var student = await _context.Users.FindAsync(dto.StudentId);
        var campaign = await _context.Campaigns.FindAsync(dto.CampaignId);

        if (student == null || campaign == null) return false;

        var existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == dto.StudentId && a.CampaignId == dto.CampaignId);

        if (existingAccount != null) return false;

        var account = new Account
        {
            UserId = dto.StudentId,
            CampaignId = dto.CampaignId,
            AccountNumber = $"ACC-{dto.StudentId}-{dto.CampaignId}-{DateTime.Now.Ticks}",
            Balance = 0,
            Status = "active"
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<CampaignResponseDto>> GetCompanyCampaignsAsync(int companyUserId)
    {
        return await _context.Campaigns
            .Where(c => c.UserId == companyUserId)
            .Include(c => c.User)
            .Include(c => c.Accounts)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CampaignType = "General",
                Budget = c.Budget ?? 0,
                RemainingBudget = c.Budget ?? 0,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Schedule = c.Schedule,
                Location = c.Location,
                ContactNumber = c.ContactNumber,
                Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count,
                CompanyName = c.User!.Company
            })
            .ToListAsync();
    }
}
