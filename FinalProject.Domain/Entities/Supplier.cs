using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

public partial class Supplier
{
    public int Id { get; set; }

    public int? SupplierTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual SupplierType? SupplierType { get; set; }
}
