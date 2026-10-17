using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Issues;

public record UpdateIssueDto(
    string? Title,
    string? Description,
    IssuePriority? Priority,
    IssueStatus? Status,
    DateTime? Deadline,
    Guid? AssigneeId,
    bool ClearAssignee = false,
    List<Guid>? TagIds = null);
