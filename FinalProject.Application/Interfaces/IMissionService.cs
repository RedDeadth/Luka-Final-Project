using FinalProject.Application.DTOs.MissionDtos;

namespace FinalProject.Application.Interfaces;

public interface IMissionService
{
    Task<bool> AssignMissionAsync(AssignMissionDto dto);
    Task<bool> CompleteMissionAsync(CompleteMissionDto dto);
    Task<List<UserMissionResponseDto>> GetUserMissionsAsync(int userId);
    Task<List<UserMissionResponseDto>> GetPendingMissionsAsync(int userId);
    Task<List<UserMissionResponseDto>> GetCompletedMissionsAsync(int userId);
}
