using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Issues.Commands;

public record MoveIssueCommand(Guid IssueId, IssueStatus NewStatus) : IRequest;

public class MoveIssueCommandHandler : IRequestHandler<MoveIssueCommand>
{
    private readonly IAppDbContext _db;

    public MoveIssueCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(MoveIssueCommand request, CancellationToken cancellationToken)
    {
        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == request.IssueId, cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");

        issue.Status = request.NewStatus;
        issue.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
