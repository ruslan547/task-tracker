using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Board : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
}
