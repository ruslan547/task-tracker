using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Application.Features.Sprints.Commands;

public record AssignIssueToSprintCommand(Guid IssueId, Guid? SprintId) : IRequest;

public class AssignIssueToSprintCommandHandler : IRequestHandler<AssignIssueToSprintCommand>
{
    private readonly IAppDbContext _db;

    public AssignIssueToSprintCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(AssignIssueToSprintCommand request, CancellationToken cancellationToken)
    {
        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == request.IssueId, cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");

        issue.SprintId = request.SprintId;
        issue.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
