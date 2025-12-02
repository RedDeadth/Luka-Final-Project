namespace FinalProject.Application.DTOs.CouponDtos;

public class CouponResponseDto
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public string CampaignName { get; set; } = null!;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool Active { get; set; }
}
