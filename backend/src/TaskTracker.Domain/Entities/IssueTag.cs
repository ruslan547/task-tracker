namespace TaskTracker.Domain.Entities;

public class IssueTag
{
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
