using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class UserIdea
{
    public int UserId { get; set; }

    public int IdeaId { get; set; }

    public bool IsWishlist { get; set; }

    public bool IsVoted { get; set; }

    public bool ReceiveUpdates { get; set; }

    public virtual Idea Idea { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
