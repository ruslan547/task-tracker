namespace TaskTracker.Application.DTOs.Auth;

public record AuthResultDto(string Token, Guid UserId, string Email, string FullName, string Role);
