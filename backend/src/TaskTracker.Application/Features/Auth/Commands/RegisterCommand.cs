using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterDto Dto) : IRequest<AuthResultDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwt;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IJwtService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            Email = request.Dto.Email,
            UserName = request.Dto.Email,
            FullName = request.Dto.FullName,
            Role = UserRole.Developer
        };

        var result = await _userManager.CreateAsync(user, request.Dto.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        var token = _jwt.GenerateToken(user);
        return new AuthResultDto(token, user.Id, user.Email!, user.FullName, user.Role.ToString());
    }
}
