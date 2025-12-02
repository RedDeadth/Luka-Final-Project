using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Features.Campaign;
using FinalProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Campaign;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<CampaignResponseDto>>
{
    private readonly LukitasDbContext _context;
    public CreateCampaignCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<CampaignResponseDto>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Users.FindAsync(new object[] { request.CompanyUserId }, cancellationToken);
        if (company == null) return Result<CampaignResponseDto>.NotFound("Company not found");

        var campaign = new Domain.Entities.Campaign
        {
            UserId = request.CompanyUserId, Name = request.Name,
            Description = $"{request.Description}\nTipo: {request.CampaignType}\nImágenes: {string.Join(", ", request.ImageUrls ?? new List<string>())}",
            Budget = request.Budget, StartDate = request.StartDate, EndDate = request.EndDate,
            Schedule = request.Schedule, Location = request.Location, ContactNumber = request.ContactNumber, Active = true
        };

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CampaignResponseDto>.Created(new CampaignResponseDto
        {
            Id = campaign.Id, Name = campaign.Name, Description = campaign.Description, CampaignType = request.CampaignType,
            Budget = campaign.Budget ?? 0, RemainingBudget = campaign.Budget ?? 0, StartDate = campaign.StartDate,
            EndDate = campaign.EndDate, Schedule = campaign.Schedule, Location = campaign.Location,
            ContactNumber = campaign.ContactNumber, Active = true, EnrolledStudents = 0, CompanyName = company.Company
        });
    }
}

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, Result<bool>>
{
    private readonly LukitasDbContext _context;
    public EnrollStudentCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Users.FindAsync(new object[] { request.StudentId }, cancellationToken);
        var campaign = await _context.Campaigns.FindAsync(new object[] { request.CampaignId }, cancellationToken);

        if (student == null || campaign == null) return Result<bool>.NotFound("Student or campaign not found");

        var exists = await _context.Accounts.AnyAsync(a => a.UserId == request.StudentId && a.CampaignId == request.CampaignId, cancellationToken);
        if (exists) return Result<bool>.Fail("Student is already enrolled");

        _context.Accounts.Add(new Account
        {
            UserId = request.StudentId, CampaignId = request.CampaignId,
            AccountNumber = $"ACC-{request.StudentId}-{request.CampaignId}-{DateTime.Now.Ticks}",
            Balance = 0, Status = "active"
        });
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
