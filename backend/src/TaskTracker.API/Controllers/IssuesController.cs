using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Issues;
using TaskTracker.Application.Features.Issues.Commands;
using TaskTracker.Application.Features.Issues.Queries;
using TaskTracker.Domain.Enums;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IssuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<List<IssueDto>>> GetByProject(Guid projectId)
    {
        var result = await _mediator.Send(new GetIssuesByProjectQuery(projectId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IssueDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetIssueByIdQuery(id));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager,Developer,Tester")]
    [HttpPost]
    public async Task<ActionResult<IssueDto>> Create(CreateIssueDto dto)
    {
        var result = await _mediator.Send(new CreateIssueCommand(dto, UserId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Administrator,ProjectManager,Developer,Tester")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IssueDto>> Update(Guid id, UpdateIssueDto dto)
    {
        var result = await _mediator.Send(new UpdateIssueCommand(id, dto));
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,ProjectManager,Developer,Tester")]
    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveIssueDto dto)
    {
        await _mediator.Send(new MoveIssueCommand(id, dto.Status));
        return NoContent();
    }

    [Authorize(Roles = "Administrator,ProjectManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteIssueCommand(id));
        return NoContent();
    }
}
