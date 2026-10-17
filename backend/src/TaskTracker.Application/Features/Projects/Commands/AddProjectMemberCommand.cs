using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Projects.Commands;

public record AddProjectMemberCommand(Guid ProjectId, AddProjectMemberDto Dto) : IRequest;

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand>
{
    private readonly IAppDbContext _db;

    public AddProjectMemberCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == request.Dto.UserId, cancellationToken);

        if (exists) return;

        var role = Enum.Parse<UserRole>(request.Dto.Role);

        _db.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = request.ProjectId,
            UserId = request.Dto.UserId,
            Role = role
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
