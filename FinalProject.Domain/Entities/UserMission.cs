using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

/// <summary>
/// Missions assigned to users with their status
/// </summary>
public partial class UserMission
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? MissionId { get; set; }

    public bool? Completed { get; set; }

    public DateOnly? AssignmentDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public int? SaleId { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual MissionTemplate? Mission { get; set; }

    public virtual Sale? Sale { get; set; }

    public virtual User? User { get; set; }
}
