using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Issues;

public record IssueDto(
    Guid Id,
    string Title,
    string? Description,
    IssueType Type,
    IssuePriority Priority,
    IssueStatus Status,
    string IssueKey,
    int IssueNumber,
    DateTime? Deadline,
    Guid ProjectId,
    Guid? AssigneeId,
    string? AssigneeName,
    Guid ReporterId,
    string ReporterName,
    Guid? ParentIssueId,
    Guid? SprintId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<string> Tags);

public record MoveIssueDto(IssueStatus Status);
