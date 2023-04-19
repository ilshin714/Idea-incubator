using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class UserIdeaRole
{
    public int UserId { get; set; }

    public int IdeaId { get; set; }

    public int RoleId { get; set; }

    public virtual Idea Idea { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
