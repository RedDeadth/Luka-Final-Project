using FinalProject.Application.DTOs.MissionDtos;

namespace FinalProject.Application.Interfaces;

public interface IMissionService
{
    Task<bool> AssignMissionAsync(AssignMissionDto dto);
    Task<bool> CompleteMissionAsync(CompleteMissionDto dto);
    IQueryable<UserMissionResponseDto> GetUserMissions(int userId);
    IQueryable<UserMissionResponseDto> GetPendingMissions(int userId);
    IQueryable<UserMissionResponseDto> GetCompletedMissions(int userId);
}
