namespace FinalProject.Application.DTOs.SupplierDtos;

public class SupplierResponseDto
{
    public int Id { get; set; }
    public int SupplierTypeId { get; set; }
    public string SupplierTypeName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = null!;
    public int TotalProducts { get; set; }
}
