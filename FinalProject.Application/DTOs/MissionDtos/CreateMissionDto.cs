namespace FinalProject.Application.DTOs.MissionDtos
{
    public class CreateMissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
    }
}