namespace FinalProject.Application.DTOs.CompanyDtos;

public class CompanyApprovalDto
{
    public int CompanyId { get; set; }
    public bool Approved { get; set; }
    public string? Reason { get; set; }
}
