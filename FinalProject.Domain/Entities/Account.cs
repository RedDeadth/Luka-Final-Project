using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

public partial class Account
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public decimal? Balance { get; set; }

    public string? Status { get; set; }

    public int? CampaignId { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<Transfer> TransferDestinationAccounts { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferSourceAccounts { get; set; } = new List<Transfer>();

    public virtual User? User { get; set; }
}
