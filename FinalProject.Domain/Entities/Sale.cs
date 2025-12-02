using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

/// <summary>
/// sale_id serves as voucher code
/// </summary>
public partial class Sale
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public DateTime? SaleDate { get; set; }

    public decimal Total { get; set; }

    public string? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    public virtual ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();
}
