using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class Content
{
    public int ContentId { get; set; }

    public int Writer { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int DisclosureLevel { get; set; }

    public int ContentType { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ContentType ContentTypeNavigation { get; set; } = null!;

    public virtual User WriterNavigation { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Idea> Ideas { get; } = new List<Idea>();
}
