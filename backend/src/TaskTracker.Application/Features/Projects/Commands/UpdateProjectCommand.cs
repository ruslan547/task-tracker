using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Features.Projects.Commands;

public record UpdateProjectCommand(Guid Id, UpdateProjectDto Dto) : IRequest<ProjectDto>;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IAppDbContext _db;

    public UpdateProjectCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _db.Projects
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");

        project.Name = request.Dto.Name;
        project.Description = request.Dto.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        var memberCount = await _db.ProjectMembers.CountAsync(m => m.ProjectId == project.Id, cancellationToken);
        var issueCount = await _db.Issues.CountAsync(i => i.ProjectId == project.Id, cancellationToken);

        return new ProjectDto(project.Id, project.Name, project.Key, project.Description,
            project.OwnerId, project.Owner.FullName, project.CreatedAt, memberCount, issueCount);
    }
}
