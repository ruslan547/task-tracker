using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<Board> Boards { get; }
    DbSet<BoardColumn> BoardColumns { get; }
    DbSet<Issue> Issues { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Tag> Tags { get; }
    DbSet<IssueTag> IssueTags { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
