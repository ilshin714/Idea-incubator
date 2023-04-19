using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class IdeaStatus
{
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Idea> Ideas { get; } = new List<Idea>();
}
