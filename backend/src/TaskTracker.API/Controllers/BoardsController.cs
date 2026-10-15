using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Boards;
using TaskTracker.Application.Features.Boards.Queries;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<BoardDto>> GetByProject(Guid projectId)
    {
        var result = await _mediator.Send(new GetBoardByProjectQuery(projectId));
        return Ok(result);
    }
}
