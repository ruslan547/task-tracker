using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Issues;

namespace TaskTracker.Application.Features.Issues.Queries;

public record GetIssuesByProjectQuery(Guid ProjectId) : IRequest<List<IssueDto>>;

public class GetIssuesByProjectQueryHandler : IRequestHandler<GetIssuesByProjectQuery, List<IssueDto>>
{
    private readonly IAppDbContext _db;

    public GetIssuesByProjectQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<IssueDto>> Handle(GetIssuesByProjectQuery request, CancellationToken cancellationToken)
    {
        return await _db.Issues
            .Where(i => i.ProjectId == request.ProjectId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new IssueDto(
                i.Id, i.Title, i.Description, i.Type, i.Priority, i.Status,
                i.IssueKey, i.IssueNumber, i.Deadline,
                i.ProjectId, i.AssigneeId, i.Assignee != null ? i.Assignee.FullName : null,
                i.ReporterId, i.Reporter.FullName, i.ParentIssueId,
                i.CreatedAt, i.UpdatedAt,
                i.IssueTags.Select(t => t.Tag.Name).ToList()))
            .ToListAsync(cancellationToken);
    }
}
