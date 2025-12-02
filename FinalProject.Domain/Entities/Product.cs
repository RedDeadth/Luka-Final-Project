using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int? SupplierId { get; set; }

    public int? ProductTypeId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int? Stock { get; set; }

    public string? Status { get; set; }

    public virtual ProductType? ProductType { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    public virtual Supplier? Supplier { get; set; }
}
