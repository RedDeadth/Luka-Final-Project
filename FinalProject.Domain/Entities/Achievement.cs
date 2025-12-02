using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

/// <summary>
/// Achievements obtained by completing missions
/// </summary>
public partial class Achievement
{
    public int Id { get; set; }

    public int? UserMissionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? AchievementDate { get; set; }

    public bool? Active { get; set; }

    public virtual UserMission? UserMission { get; set; }
}
