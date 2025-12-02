namespace FinalProject.Application.DTOs.ProductDtos;

public class UpdateProductDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = null!;
}
