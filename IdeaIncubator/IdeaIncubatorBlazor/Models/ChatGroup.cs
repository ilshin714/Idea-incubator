using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class ChatGroup
{
    public int ChatGroupId { get; set; }

    public int IdeaId { get; set; }

    public virtual Idea Idea { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
