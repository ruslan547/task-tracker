using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Comment : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public AppUser Author { get; set; } = null!;
}
