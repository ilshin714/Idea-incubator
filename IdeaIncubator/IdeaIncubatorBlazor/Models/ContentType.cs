using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class ContentType
{
    public int TypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Content> Contents { get; } = new List<Content>();
}
