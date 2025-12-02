namespace FinalProject.Application.DTOs.CompanyDtos;

public class CompanyProfileDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public bool Approved { get; set; }
    public decimal LukasBalance { get; set; }
    public int ActiveCampaigns { get; set; }
}
