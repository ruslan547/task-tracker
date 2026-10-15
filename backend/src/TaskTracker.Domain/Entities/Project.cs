using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public AppUser Owner { get; set; } = null!;

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
