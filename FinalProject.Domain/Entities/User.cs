using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

/// <summary>
/// Educational system users
/// </summary>
public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? StudentCode { get; set; }

    public int? RoleId { get; set; }

    public bool? Active { get; set; }

    public string Company { get; set; } = null!;

    public string University { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();
}
