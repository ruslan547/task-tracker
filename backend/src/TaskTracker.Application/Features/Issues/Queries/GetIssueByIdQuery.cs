using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Issues;

namespace TaskTracker.Application.Features.Issues.Queries;

public record GetIssueByIdQuery(Guid Id) : IRequest<IssueDto>;

public class GetIssueByIdQueryHandler : IRequestHandler<GetIssueByIdQuery, IssueDto>
{
    private readonly IAppDbContext _db;

    public GetIssueByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IssueDto> Handle(GetIssueByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Issues
            .Where(i => i.Id == request.Id)
            .Select(i => new IssueDto(
                i.Id, i.Title, i.Description, i.Type, i.Priority, i.Status,
                i.IssueKey, i.IssueNumber, i.Deadline,
                i.ProjectId, i.AssigneeId, i.Assignee != null ? i.Assignee.FullName : null,
                i.ReporterId, i.Reporter.FullName, i.ParentIssueId, i.SprintId,
                i.CreatedAt, i.UpdatedAt,
                i.IssueTags.Select(t => t.Tag.Name).ToList()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");
    }
}
