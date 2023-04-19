using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int? ChatGroupId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string? MessageText { get; set; }

    public bool IsCurrentUser { get; set; }

    public DateTime? DateSent { get; set; }

    public virtual ChatGroup? ChatGroup { get; set; }

    public virtual User User { get; set; } = null!;
}
