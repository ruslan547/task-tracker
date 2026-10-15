using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IAppDbContext _db;

    public GetProjectByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Projects
            .Where(p => p.Id == request.Id)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Key, p.Description,
                p.OwnerId, p.Owner.FullName, p.CreatedAt,
                p.Members.Count, p.Issues.Count))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");
    }
}
