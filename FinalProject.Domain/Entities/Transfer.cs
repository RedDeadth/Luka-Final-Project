using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

public partial class Transfer
{
    public int Id { get; set; }

    public int? SourceAccountId { get; set; }

    public int? DestinationAccountId { get; set; }

    public DateOnly TransferDate { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public virtual Account? DestinationAccount { get; set; }

    public virtual Account? SourceAccount { get; set; }
}
