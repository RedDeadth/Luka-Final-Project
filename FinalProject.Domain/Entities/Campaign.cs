using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

/// <summary>
/// Campaigns promoted by coordinators
/// </summary>
public partial class Campaign
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Schedule { get; set; }

    public string? Location { get; set; }

    public string? ContactNumber { get; set; }

    public decimal? Budget { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual User? User { get; set; }
}
