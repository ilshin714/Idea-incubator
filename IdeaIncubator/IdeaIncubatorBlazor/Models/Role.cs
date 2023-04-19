using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class Role
{
    public string? RoleTitle { get; set; }

    public int? PrivilegeLevel { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<UserIdeaRole> UserIdeaRoles { get; } = new List<UserIdeaRole>();
}
