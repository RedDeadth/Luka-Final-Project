using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

public partial class ProductType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Active { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
