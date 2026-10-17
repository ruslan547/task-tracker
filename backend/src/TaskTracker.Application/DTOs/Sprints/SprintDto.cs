using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Sprints;

public record SprintDto(
    Guid Id,
    string Name,
    string? Goal,
    DateTime StartDate,
    DateTime EndDate,
    SprintStatus Status,
    Guid ProjectId,
    int TotalIssues,
    int DoneIssues);

public record CreateSprintDto(string Name, string? Goal, DateTime StartDate, DateTime EndDate);

public record BurndownPointDto(DateTime Date, int Remaining);

public record AssignSprintDto(Guid? SprintId);
