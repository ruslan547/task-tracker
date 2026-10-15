using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Boards;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Boards.Queries;

public record GetBoardByProjectQuery(Guid ProjectId, Guid? SprintId) : IRequest<BoardDto>;

public class GetBoardByProjectQueryHandler : IRequestHandler<GetBoardByProjectQuery, BoardDto>
{
    private readonly IAppDbContext _db;

    public GetBoardByProjectQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BoardDto> Handle(GetBoardByProjectQuery request, CancellationToken cancellationToken)
    {
        var sprintId = request.SprintId;
        if (sprintId == null)
        {
            sprintId = await _db.Sprints
                .Where(s => s.ProjectId == request.ProjectId && s.Status == SprintStatus.Active)
                .Select(s => (Guid?)s.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var board = await _db.Boards
            .Where(b => b.ProjectId == request.ProjectId)
            .Select(b => new BoardDto(
                b.Id, b.Name, b.ProjectId,
                b.Columns.OrderBy(c => c.Order).Select(c => new BoardColumnDto(
                    c.Id, c.Name, c.Order, c.MappedStatus.ToString(),
                    _db.Issues
                        .Where(i => i.ProjectId == request.ProjectId
                            && i.Status == c.MappedStatus
                            && (sprintId == null || i.SprintId == sprintId))
                        .Select(i => new BoardIssueDto(
                            i.Id, i.Title, i.IssueKey, i.Type.ToString(), i.Priority.ToString(),
                            i.Assignee != null ? i.Assignee.FullName : null, i.AssigneeId))
                        .ToList()
                )).ToList()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Board not found.");

        return board;
    }
}
