using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Application.Features.Projects.Commands;

public record DeleteProjectCommand(Guid Id) : IRequest;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IAppDbContext _db;

    public DeleteProjectCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
