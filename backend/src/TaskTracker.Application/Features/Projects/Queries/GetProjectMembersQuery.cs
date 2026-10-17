using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Features.Projects.Queries;

public record GetProjectMembersQuery(Guid ProjectId) : IRequest<List<ProjectMemberDto>>;

public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberDto>>
{
    private readonly IAppDbContext _db;

    public GetProjectMembersQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<ProjectMemberDto>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
    {
        return await _db.ProjectMembers
            .Where(pm => pm.ProjectId == request.ProjectId)
            .Select(pm => new ProjectMemberDto(
                pm.UserId,
                pm.User.FullName,
                pm.User.Email!,
                pm.Role.ToString()))
            .ToListAsync(cancellationToken);
    }
}
