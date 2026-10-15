namespace TaskTracker.Application.DTOs.Projects;

public record CreateProjectDto(string Name, string Key, string? Description);
