using System;
using System.Collections.Generic;

namespace FinalProject.Infrastructure;

/// <summary>
/// Master catalog of system missions
/// </summary>
public partial class MissionTemplate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? RewardPoints { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();
}
