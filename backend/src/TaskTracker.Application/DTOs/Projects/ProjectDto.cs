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

public record ProjectMemberDto(Guid UserId, string FullName, string Email, string Role);

public record AddProjectMemberDto(Guid UserId, string Role);
