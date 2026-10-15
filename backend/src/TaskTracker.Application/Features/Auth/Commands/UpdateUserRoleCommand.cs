using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Auth.Commands;

public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<UserDto>;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, UserDto>
{
    private readonly UserManager<AppUser> _userManager;

    public UpdateUserRoleCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<UserRole>(request.Role, out var role))
            throw new ArgumentException("Invalid role.");

        user.Role = role;
        await _userManager.UpdateAsync(user);

        return new UserDto(user.Id, user.Email!, user.FullName, user.Role.ToString());
    }
}
