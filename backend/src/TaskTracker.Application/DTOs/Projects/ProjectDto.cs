namespace TaskTracker.Application.DTOs.Projects;

public record ProjectDto(
    Guid Id,
    string Name,
    string Key,
    string? Description,
    Guid OwnerId,
    string OwnerName,
    DateTime CreatedAt,
    int MemberCount,
    int IssueCount);
