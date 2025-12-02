using System;
using System.Collections.Generic;

namespace FinalProject.Domain.Entities;

/// <summary>
/// System roles: student and coordinator
/// </summary>
public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Permissions { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
