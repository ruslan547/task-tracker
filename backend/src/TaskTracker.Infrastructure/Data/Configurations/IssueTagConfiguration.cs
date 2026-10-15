using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Data.Configurations;

public class IssueTagConfiguration : IEntityTypeConfiguration<IssueTag>
{
    public void Configure(EntityTypeBuilder<IssueTag> builder)
    {
        builder.HasKey(it => new { it.IssueId, it.TagId });

        builder.HasOne(it => it.Issue)
            .WithMany(i => i.IssueTags)
            .HasForeignKey(it => it.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(it => it.Tag)
            .WithMany(t => t.IssueTags)
            .HasForeignKey(it => it.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
