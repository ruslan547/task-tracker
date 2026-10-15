using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Issues;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Issues.Commands;

public record UpdateIssueCommand(Guid Id, UpdateIssueDto Dto) : IRequest<IssueDto>;

public class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand, IssueDto>
{
    private readonly IAppDbContext _db;

    public UpdateIssueCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IssueDto> Handle(UpdateIssueCommand request, CancellationToken cancellationToken)
    {
        var issue = await _db.Issues
            .Include(i => i.IssueTags)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");

        if (request.Dto.Title != null) issue.Title = request.Dto.Title;
        if (request.Dto.Description != null) issue.Description = request.Dto.Description;
        if (request.Dto.Priority.HasValue) issue.Priority = request.Dto.Priority.Value;
        if (request.Dto.Status.HasValue) issue.Status = request.Dto.Status.Value;
        if (request.Dto.Deadline.HasValue) issue.Deadline = request.Dto.Deadline.Value;
        issue.AssigneeId = request.Dto.AssigneeId ?? issue.AssigneeId;
        issue.UpdatedAt = DateTime.UtcNow;

        if (request.Dto.TagIds != null)
        {
            var existing = issue.IssueTags.ToList();
            _db.IssueTags.RemoveRange(existing);
            foreach (var tagId in request.Dto.TagIds)
            {
                _db.IssueTags.Add(new IssueTag { IssueId = issue.Id, TagId = tagId });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return await _db.Issues
            .Where(i => i.Id == issue.Id)
            .Select(i => new IssueDto(
                i.Id, i.Title, i.Description, i.Type, i.Priority, i.Status,
                i.IssueKey, i.IssueNumber, i.Deadline,
                i.ProjectId, i.AssigneeId, i.Assignee != null ? i.Assignee.FullName : null,
                i.ReporterId, i.Reporter.FullName, i.ParentIssueId,
                i.CreatedAt, i.UpdatedAt,
                i.IssueTags.Select(t => t.Tag.Name).ToList()))
            .FirstAsync(cancellationToken);
    }
}
