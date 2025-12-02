namespace FinalProject.Application.DTOs.MissionDtos;

public class UserMissionResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int MissionId { get; set; }
    public string MissionName { get; set; } = null!;
    public string MissionDescription { get; set; } = null!;
    public int RewardPoints { get; set; }
    public bool Completed { get; set; }
    public DateOnly? AssignmentDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
}
