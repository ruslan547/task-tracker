namespace TaskTracker.Application.DTOs.Auth;

public record UserDto(Guid Id, string Email, string FullName, string Role);
