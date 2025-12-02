using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.MissionDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class MissionService : IMissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public MissionService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<bool> AssignMissionAsync(AssignMissionDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
        var mission = await _unitOfWork.MissionTemplates.GetByIdAsync(dto.MissionId);

        if (user == null || mission == null) return false;

        var userMission = new UserMission
        {
            UserId = dto.UserId,
            MissionId = dto.MissionId,
            Completed = false,
            AssignmentDate = DateOnly.FromDateTime(DateTime.Now)
        };

        await _unitOfWork.UserMissions.AddAsync(userMission);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CompleteMissionAsync(CompleteMissionDto dto)
    {
        var userMission = await _unitOfWork.UserMissions.GetByIdAsync(dto.UserMissionId);
        if (userMission == null) return false;

        userMission.Completed = true;
        userMission.CompletionDate = DateOnly.FromDateTime(DateTime.Now);
        userMission.SaleId = dto.SaleId;

        var mission = await _unitOfWork.MissionTemplates.GetByIdAsync(userMission.MissionId ?? 0);
        if (mission != null && mission.RewardPoints > 0)
        {
            var userAccount = await _unitOfWork.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userMission.UserId && a.Status == "active");

            if (userAccount != null)
            {
                userAccount.Balance += mission.RewardPoints;
                _unitOfWork.Accounts.Update(userAccount);
            }
        }

        _unitOfWork.UserMissions.Update(userMission);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public IQueryable<UserMissionResponseDto> GetUserMissions(int userId)
    {
        return _context.UserMissions
            .Include(um => um.User)
            .Include(um => um.Mission)
            .Where(um => um.UserId == userId)
            .Select(um => new UserMissionResponseDto
            {
                Id = um.Id,
                UserId = um.UserId ?? 0,
                UserName = $"{um.User!.FirstName} {um.User.LastName}",
                MissionId = um.MissionId ?? 0,
                MissionName = um.Mission!.Name,
                MissionDescription = um.Mission.Description ?? "",
                RewardPoints = um.Mission.RewardPoints ?? 0,
                Completed = um.Completed ?? false,
                AssignmentDate = um.AssignmentDate,
                CompletionDate = um.CompletionDate
            });
    }

    public IQueryable<UserMissionResponseDto> GetPendingMissions(int userId)
    {
        return _context.UserMissions
            .Include(um => um.User)
            .Include(um => um.Mission)
            .Where(um => um.UserId == userId && um.Completed == false)
            .Select(um => new UserMissionResponseDto
            {
                Id = um.Id,
                UserId = um.UserId ?? 0,
                UserName = $"{um.User!.FirstName} {um.User.LastName}",
                MissionId = um.MissionId ?? 0,
                MissionName = um.Mission!.Name,
                MissionDescription = um.Mission.Description ?? "",
                RewardPoints = um.Mission.RewardPoints ?? 0,
                Completed = um.Completed ?? false,
                AssignmentDate = um.AssignmentDate,
                CompletionDate = um.CompletionDate
            });
    }

    public IQueryable<UserMissionResponseDto> GetCompletedMissions(int userId)
    {
        return _context.UserMissions
            .Include(um => um.User)
            .Include(um => um.Mission)
            .Where(um => um.UserId == userId && um.Completed == true)
            .Select(um => new UserMissionResponseDto
            {
                Id = um.Id,
                UserId = um.UserId ?? 0,
                UserName = $"{um.User!.FirstName} {um.User.LastName}",
                MissionId = um.MissionId ?? 0,
                MissionName = um.Mission!.Name,
                MissionDescription = um.Mission.Description ?? "",
                RewardPoints = um.Mission.RewardPoints ?? 0,
                Completed = um.Completed ?? false,
                AssignmentDate = um.AssignmentDate,
                CompletionDate = um.CompletionDate
            });
    }
}
