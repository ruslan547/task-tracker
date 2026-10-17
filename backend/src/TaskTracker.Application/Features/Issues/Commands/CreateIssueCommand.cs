using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Issues;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Issues.Commands;

public record CreateIssueCommand(CreateIssueDto Dto, Guid ReporterId) : IRequest<IssueDto>;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly IAppDbContext _db;

    public CreateIssueCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Dto.ProjectId, cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");

        var maxNumber = await _db.Issues
            .Where(i => i.ProjectId == request.Dto.ProjectId)
            .MaxAsync(i => (int?)i.IssueNumber, cancellationToken) ?? 0;

        var issueNumber = maxNumber + 1;

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            Title = request.Dto.Title,
            Description = request.Dto.Description,
            Type = request.Dto.Type,
            Priority = request.Dto.Priority,
            Status = IssueStatus.Todo,
            Deadline = request.Dto.Deadline,
            ProjectId = request.Dto.ProjectId,
            AssigneeId = request.Dto.AssigneeId,
            ReporterId = request.ReporterId,
            ParentIssueId = request.Dto.ParentIssueId,
            SprintId = request.Dto.SprintId,
            IssueNumber = issueNumber,
            IssueKey = $"{project.Key}-{issueNumber}"
        };

        _db.Issues.Add(issue);

        if (request.Dto.TagIds?.Count > 0)
        {
            foreach (var tagId in request.Dto.TagIds)
            {
                _db.IssueTags.Add(new IssueTag { IssueId = issue.Id, TagId = tagId });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return await GetIssueDtoById(issue.Id, cancellationToken);
    }

    private async Task<IssueDto> GetIssueDtoById(Guid id, CancellationToken ct)
    {
        return await _db.Issues
            .Where(i => i.Id == id)
            .Select(i => new IssueDto(
                i.Id, i.Title, i.Description, i.Type, i.Priority, i.Status,
                i.IssueKey, i.IssueNumber, i.Deadline,
                i.ProjectId, i.AssigneeId, i.Assignee != null ? i.Assignee.FullName : null,
                i.ReporterId, i.Reporter.FullName, i.ParentIssueId, i.SprintId,
                i.CreatedAt, i.UpdatedAt,
                i.IssueTags.Select(t => t.Tag.Name).ToList()))
            .FirstAsync(ct);
    }
}
