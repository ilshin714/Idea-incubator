using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class Idea
{
    public int IdeaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Status { get; set; }

    public int Vote { get; set; }

    public string? Keywords { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<ChatGroup> ChatGroups { get; } = new List<ChatGroup>();

    public virtual IdeaStatus StatusNavigation { get; set; } = null!;

    public virtual ICollection<UserIdeaRole> UserIdeaRoles { get; } = new List<UserIdeaRole>();

    public virtual ICollection<UserIdea> UserIdeas { get; } = new List<UserIdea>();

    public virtual ICollection<Content> Contents { get; } = new List<Content>();
}
