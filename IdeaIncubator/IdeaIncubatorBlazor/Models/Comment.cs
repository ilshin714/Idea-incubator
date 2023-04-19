namespace IdeaIncubatorBlazor.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual User UserIdNavigation { get; set; } = null!;

    public virtual ICollection<Content> Contents { get; } = new List<Content>();
}
