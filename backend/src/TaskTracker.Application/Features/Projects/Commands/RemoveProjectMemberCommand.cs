using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Application.Features.Projects.Commands;

public record RemoveProjectMemberCommand(Guid ProjectId, Guid UserId) : IRequest;

public class RemoveProjectMemberCommandHandler : IRequestHandler<RemoveProjectMemberCommand>
{
    private readonly IAppDbContext _db;

    public RemoveProjectMemberCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == request.UserId, cancellationToken);

        if (member == null) return;

        _db.ProjectMembers.Remove(member);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
