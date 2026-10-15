using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Issue : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IssueType Type { get; set; }
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    public IssueStatus Status { get; set; } = IssueStatus.Todo;
    public DateTime? Deadline { get; set; }
    public string IssueKey { get; set; } = string.Empty;
    public int IssueNumber { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? AssigneeId { get; set; }
    public AppUser? Assignee { get; set; }

    public Guid ReporterId { get; set; }
    public AppUser Reporter { get; set; } = null!;

    public Guid? ParentIssueId { get; set; }
    public Issue? ParentIssue { get; set; }
    public ICollection<Issue> SubIssues { get; set; } = new List<Issue>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
}
