using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Comments;
using TaskTracker.Application.Features.Comments.Commands;
using TaskTracker.Application.Features.Comments.Queries;

namespace TaskTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("issue/{issueId:guid}")]
    public async Task<ActionResult<List<CommentDto>>> GetByIssue(Guid issueId)
    {
        var result = await _mediator.Send(new GetCommentsByIssueQuery(issueId));
        return Ok(result);
    }

    [HttpPost("issue/{issueId:guid}")]
    public async Task<ActionResult<CommentDto>> Create(Guid issueId, CreateCommentDto dto)
    {
        var result = await _mediator.Send(new CreateCommentCommand(issueId, dto.Text, UserId));
        return Ok(result);
    }
}
