using System;
using System.Collections.Generic;

namespace IdeaIncubatorBlazor.Models;

public partial class User
{
    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? IsValidEmail { get; set; }

    public string? UserName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime JoinDate { get; set; }

    public bool? IsProfileVisible { get; set; }

    public string? SkillSets { get; set; }

    public System.Guid GUID { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Content> Contents { get; } = new List<Content>();

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<UserIdeaRole> UserIdeaRoles { get; } = new List<UserIdeaRole>();

    public virtual ICollection<UserIdea> UserIdeas { get; } = new List<UserIdea>();

    public virtual ICollection<ChatGroup> ChatGroups { get; } = new List<ChatGroup>();
}
