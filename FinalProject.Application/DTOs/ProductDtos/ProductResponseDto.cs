namespace FinalProject.Application.DTOs.ProductDtos;

public class ProductResponseDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public int ProductTypeId { get; set; }
    public string ProductTypeName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = null!;
}
