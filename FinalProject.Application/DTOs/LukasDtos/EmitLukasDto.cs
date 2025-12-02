namespace FinalProject.Application.DTOs.LukasDtos;

public class EmitLukasDto
{
    public int CompanyId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = null!;
}
