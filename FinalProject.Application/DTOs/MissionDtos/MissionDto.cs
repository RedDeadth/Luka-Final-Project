namespace FinalProject.Application.DTOs.MissionDtos
{
    public class MissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public bool Completed { get; set; }
    }
}