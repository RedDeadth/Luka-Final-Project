namespace FinalProject.Application.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SupplierId { get; set; }
        public int ProductTypeId { get; set; }
    }
}