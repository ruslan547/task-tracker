using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Comments;

namespace TaskTracker.Application.Features.Comments.Queries;

public record GetCommentsByIssueQuery(Guid IssueId) : IRequest<List<CommentDto>>;

public class GetCommentsByIssueQueryHandler : IRequestHandler<GetCommentsByIssueQuery, List<CommentDto>>
{
    private readonly IAppDbContext _db;

    public GetCommentsByIssueQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByIssueQuery request, CancellationToken cancellationToken)
    {
        return await _db.Comments
            .Where(c => c.IssueId == request.IssueId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(c.Id, c.Text, c.AuthorId, c.Author.FullName, c.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
