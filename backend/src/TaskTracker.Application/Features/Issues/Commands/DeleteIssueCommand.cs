using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Application.Features.Issues.Commands;

public record DeleteIssueCommand(Guid Id) : IRequest;

public class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand>
{
    private readonly IAppDbContext _db;

    public DeleteIssueCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteIssueCommand request, CancellationToken cancellationToken)
    {
        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");

        _db.Issues.Remove(issue);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
