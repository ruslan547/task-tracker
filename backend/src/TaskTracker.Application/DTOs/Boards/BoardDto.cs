namespace TaskTracker.Application.DTOs.Boards;

public record BoardDto(Guid Id, string Name, Guid ProjectId, List<BoardColumnDto> Columns);

public record BoardColumnDto(Guid Id, string Name, int Order, string MappedStatus, List<BoardIssueDto> Issues);

public record BoardIssueDto(
    Guid Id,
    string Title,
    string IssueKey,
    string Type,
    string Priority,
    string? AssigneeName,
    Guid? AssigneeId);
