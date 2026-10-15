using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Data.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Title).HasMaxLength(500).IsRequired();
        builder.Property(i => i.IssueKey).HasMaxLength(20).IsRequired();
        builder.HasIndex(i => i.IssueKey).IsUnique();

        builder.HasOne(i => i.Assignee)
            .WithMany(u => u.AssignedIssues)
            .HasForeignKey(i => i.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Reporter)
            .WithMany()
            .HasForeignKey(i => i.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ParentIssue)
            .WithMany(i => i.SubIssues)
            .HasForeignKey(i => i.ParentIssueId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
