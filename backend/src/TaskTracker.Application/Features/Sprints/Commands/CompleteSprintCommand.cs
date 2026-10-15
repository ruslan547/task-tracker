using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Sprints.Commands;

public record CompleteSprintCommand(Guid SprintId) : IRequest<SprintDto>;

public class CompleteSprintCommandHandler : IRequestHandler<CompleteSprintCommand, SprintDto>
{
    private readonly IAppDbContext _db;

    public CompleteSprintCommandHandler(IAppDbContext db) => _db = db;

    public async Task<SprintDto> Handle(CompleteSprintCommand request, CancellationToken cancellationToken)
    {
        var sprint = await _db.Sprints
            .FirstOrDefaultAsync(s => s.Id == request.SprintId, cancellationToken)
            ?? throw new KeyNotFoundException("Sprint not found.");

        sprint.Status = SprintStatus.Completed;
        sprint.UpdatedAt = DateTime.UtcNow;

        var unfinished = await _db.Issues
            .Where(i => i.SprintId == sprint.Id && i.Status != IssueStatus.Done)
            .ToListAsync(cancellationToken);

        foreach (var issue in unfinished)
            issue.SprintId = null;

        await _db.SaveChangesAsync(cancellationToken);

        var total = await _db.Issues.CountAsync(i => i.SprintId == sprint.Id, cancellationToken);

        return new SprintDto(sprint.Id, sprint.Name, sprint.Goal, sprint.StartDate,
            sprint.EndDate, sprint.Status, sprint.ProjectId, total, total);
    }
}
