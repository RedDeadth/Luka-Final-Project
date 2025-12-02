namespace FinalProject.Application.DTOs.ProductDtos;

public class ProductPurchaseDto
{
    public int StudentId { get; set; }
    public int SupplierId { get; set; }
    public List<ProductItemDto> Items { get; set; } = new();
}

public class ProductItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
