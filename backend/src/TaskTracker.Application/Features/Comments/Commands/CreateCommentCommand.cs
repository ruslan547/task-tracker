using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs.Comments;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Comments.Commands;

public record CreateCommentCommand(Guid IssueId, string Text, Guid AuthorId) : IRequest<CommentDto>;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly IAppDbContext _db;

    public CreateCommentCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var issue = await _db.Issues
            .FirstOrDefaultAsync(i => i.Id == request.IssueId, cancellationToken)
            ?? throw new KeyNotFoundException("Issue not found.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            IssueId = request.IssueId,
            AuthorId = request.AuthorId
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);

        var author = await _db.Comments
            .Where(c => c.Id == comment.Id)
            .Select(c => c.Author.FullName)
            .FirstAsync(cancellationToken);

        return new CommentDto(comment.Id, comment.Text, comment.AuthorId, author, comment.CreatedAt);
    }
}
