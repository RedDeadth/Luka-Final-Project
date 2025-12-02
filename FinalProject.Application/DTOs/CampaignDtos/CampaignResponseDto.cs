namespace FinalProject.Application.DTOs.CampaignDtos;

public class CampaignResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CampaignType { get; set; } = null!;
    public decimal Budget { get; set; }
    public decimal RemainingBudget { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Schedule { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public List<string>? ImageUrls { get; set; }
    public bool Active { get; set; }
    public int EnrolledStudents { get; set; }
    public string CompanyName { get; set; } = null!;
}
