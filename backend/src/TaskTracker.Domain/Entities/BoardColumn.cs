using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardColumn : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public IssueStatus MappedStatus { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;
}
