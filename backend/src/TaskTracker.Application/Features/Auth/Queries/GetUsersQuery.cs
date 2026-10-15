using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Auth.Queries;

public record GetUsersQuery : IRequest<List<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUsersQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userManager.Users
            .Select(u => new UserDto(u.Id, u.Email!, u.FullName, u.Role.ToString()))
            .ToListAsync(cancellationToken);
    }
}
