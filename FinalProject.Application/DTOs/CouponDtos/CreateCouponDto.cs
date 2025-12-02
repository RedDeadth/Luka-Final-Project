namespace FinalProject.Application.DTOs.CouponDtos;

public class CreateCouponDto
{
    public int CampaignId { get; set; }
    public int SupplierId { get; set; }
    public string Code { get; set; } = null!;
    public string DiscountType { get; set; } = null!; // 'percentage' o 'fixed_amount'
    public decimal DiscountValue { get; set; }
    public DateOnly ExpirationDate { get; set; }
}
