using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Application.Features.Sprints.Commands;
using TaskTracker.Application.Features.Sprints.Queries;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SprintsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SprintsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<List<SprintDto>>> GetByProject(Guid projectId)
    {
        var result = await _mediator.Send(new GetSprintsByProjectQuery(projectId));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPost("project/{projectId:guid}")]
    public async Task<ActionResult<SprintDto>> Create(Guid projectId, CreateSprintDto dto)
    {
        var result = await _mediator.Send(new CreateSprintCommand(projectId, dto));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPatch("{id:guid}/start")]
    public async Task<ActionResult<SprintDto>> Start(Guid id)
    {
        var result = await _mediator.Send(new StartSprintCommand(id));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<SprintDto>> Complete(Guid id)
    {
        var result = await _mediator.Send(new CompleteSprintCommand(id));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager,Developer,Tester")]
    [HttpPatch("assign/{issueId:guid}")]
    public async Task<IActionResult> AssignIssue(Guid issueId, [FromBody] Guid? sprintId)
    {
        await _mediator.Send(new AssignIssueToSprintCommand(issueId, sprintId));
        return NoContent();
    }

    [HttpGet("{id:guid}/burndown")]
    public async Task<ActionResult<List<BurndownPointDto>>> Burndown(Guid id)
    {
        var result = await _mediator.Send(new GetBurndownQuery(id));
        return Ok(result);
    }
}
