namespace FinalProject.Application.DTOs.MissionDtos
{
    public class CompleteMissionDto
    {
        public int MissionId { get; set; }
        public int UserId { get; set; }
        public DateTime CompletionDate { get; set; } = DateTime.Now;
    }
}