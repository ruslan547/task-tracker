using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Sprints.Queries;

public record GetBurndownQuery(Guid SprintId) : IRequest<List<BurndownPointDto>>;

public class GetBurndownQueryHandler : IRequestHandler<GetBurndownQuery, List<BurndownPointDto>>
{
    private readonly IAppDbContext _db;

    public GetBurndownQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<BurndownPointDto>> Handle(GetBurndownQuery request, CancellationToken cancellationToken)
    {
        var sprint = await _db.Sprints
            .FirstOrDefaultAsync(s => s.Id == request.SprintId, cancellationToken)
            ?? throw new KeyNotFoundException("Sprint not found.");

        var issues = await _db.Issues
            .Where(i => i.SprintId == sprint.Id)
            .Select(i => new { i.Status, i.UpdatedAt, i.CreatedAt })
            .ToListAsync(cancellationToken);

        var totalIssues = issues.Count;
        var points = new List<BurndownPointDto>();
        var endDate = sprint.Status == SprintStatus.Completed ? sprint.EndDate : DateTime.UtcNow;
        if (endDate > DateTime.UtcNow) endDate = DateTime.UtcNow;

        for (var date = sprint.StartDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var doneByDate = issues.Count(i =>
                i.Status == IssueStatus.Done &&
                (i.UpdatedAt ?? i.CreatedAt) <= date.AddDays(1));
            points.Add(new BurndownPointDto(date, totalIssues - doneByDate));
        }

        return points;
    }
}
