using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Auth.Commands;

public record LoginCommand(LoginDto Dto) : IRequest<AuthResultDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(UserManager<AppUser> userManager, IJwtService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwt.GenerateToken(user);
        return new AuthResultDto(token, user.Id, user.Email!, user.FullName, user.Role.ToString());
    }
}
