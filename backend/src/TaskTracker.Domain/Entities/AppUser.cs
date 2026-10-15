using Microsoft.AspNetCore.Identity;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Developer;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
    public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
