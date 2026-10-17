using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Issues;

public record CreateIssueDto(
    string Title,
    string? Description,
    IssueType Type,
    IssuePriority Priority,
    DateTime? Deadline,
    Guid ProjectId,
    Guid? AssigneeId,
    Guid? ParentIssueId,
    Guid? SprintId,
    List<Guid>? TagIds);
