using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

public partial class Coupon
{
    public int Id { get; set; }

    public int? CampaignId { get; set; }

    public int? SupplierId { get; set; }

    public string Code { get; set; } = null!;

    public string? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public bool? Active { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
