using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Features.Projects.Commands;
using TaskTracker.Application.Features.Projects.Queries;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetProjectsQuery(UserId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var result = await _mediator.Send(new CreateProjectCommand(dto, UserId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, UpdateProjectDto dto)
    {
        var result = await _mediator.Send(new UpdateProjectCommand(id, dto));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProjectCommand(id));
        return NoContent();
    }

    [HttpGet("{projectId:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid projectId)
    {
        var result = await _mediator.Send(new GetProjectMembersQuery(projectId));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPost("{projectId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid projectId, AddProjectMemberDto dto)
    {
        await _mediator.Send(new AddProjectMemberCommand(projectId, dto));
        return NoContent();
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid projectId, Guid userId)
    {
        await _mediator.Send(new RemoveProjectMemberCommand(projectId, userId));
        return NoContent();
    }
}
