using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Sprints;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Sprints.Commands;

public record StartSprintCommand(Guid SprintId) : IRequest<SprintDto>;

public class StartSprintCommandHandler : IRequestHandler<StartSprintCommand, SprintDto>
{
    private readonly IAppDbContext _db;

    public StartSprintCommandHandler(IAppDbContext db) => _db = db;

    public async Task<SprintDto> Handle(StartSprintCommand request, CancellationToken cancellationToken)
    {
        var sprint = await _db.Sprints
            .FirstOrDefaultAsync(s => s.Id == request.SprintId, cancellationToken)
            ?? throw new KeyNotFoundException("Sprint not found.");

        sprint.Status = SprintStatus.Active;
        sprint.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var total = await _db.Issues.CountAsync(i => i.SprintId == sprint.Id, cancellationToken);
        var done = await _db.Issues.CountAsync(i => i.SprintId == sprint.Id && i.Status == IssueStatus.Done, cancellationToken);

        return new SprintDto(sprint.Id, sprint.Name, sprint.Goal, sprint.StartDate,
            sprint.EndDate, sprint.Status, sprint.ProjectId, total, done);
    }
}
