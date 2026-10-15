using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Features.Projects.Queries;

public record GetProjectsQuery(Guid UserId) : IRequest<List<ProjectDto>>;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    private readonly IAppDbContext _db;

    public GetProjectsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Projects
            .Where(p => p.OwnerId == request.UserId ||
                        p.Members.Any(m => m.UserId == request.UserId))
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Key, p.Description,
                p.OwnerId, p.Owner.FullName, p.CreatedAt,
                p.Members.Count, p.Issues.Count))
            .ToListAsync(cancellationToken);
    }
}
