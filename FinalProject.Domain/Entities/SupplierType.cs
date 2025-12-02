using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

public partial class SupplierType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Active { get; set; }

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
