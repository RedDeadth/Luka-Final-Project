namespace FinalProject.Application.DTOs.SupplierDtos;

public class CreateSupplierDto
{
    public int SupplierTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
