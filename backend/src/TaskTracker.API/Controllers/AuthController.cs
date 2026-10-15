using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.Features.Auth.Commands;
using TaskTracker.Application.Features.Auth.Queries;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var result = await _mediator.Send(new RegisterCommand(dto));
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
    {
        var result = await _mediator.Send(new LoginCommand(dto));
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserDto> Me()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var name = User.FindFirstValue(ClaimTypes.Name)!;
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        return Ok(new UserDto(userId, email, name, role));
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch("users/{userId:guid}/role")]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid userId, UpdateRoleDto dto)
    {
        var result = await _mediator.Send(new UpdateUserRoleCommand(userId, dto.Role));
        return Ok(result);
    }
}
