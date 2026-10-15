using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Sprints.Queries;

public record GetSprintsByProjectQuery(Guid ProjectId) : IRequest<List<SprintDto>>;

public class GetSprintsByProjectQueryHandler : IRequestHandler<GetSprintsByProjectQuery, List<SprintDto>>
{
    private readonly IAppDbContext _db;

    public GetSprintsByProjectQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<SprintDto>> Handle(GetSprintsByProjectQuery request, CancellationToken cancellationToken)
    {
        var sprints = await _db.Sprints
            .Where(s => s.ProjectId == request.ProjectId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new List<SprintDto>();
        foreach (var s in sprints)
        {
            var total = await _db.Issues.CountAsync(i => i.SprintId == s.Id, cancellationToken);
            var done = await _db.Issues.CountAsync(i => i.SprintId == s.Id && i.Status == IssueStatus.Done, cancellationToken);
            result.Add(new SprintDto(s.Id, s.Name, s.Goal, s.StartDate, s.EndDate, s.Status, s.ProjectId, total, done));
        }

        return result;
    }
}
