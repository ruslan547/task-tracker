using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3b82f6";
    public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
}
