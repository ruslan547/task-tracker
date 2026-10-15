namespace TaskTracker.Application.DTOs.Comments;

public record CommentDto(Guid Id, string Text, Guid AuthorId, string AuthorName, DateTime CreatedAt);
