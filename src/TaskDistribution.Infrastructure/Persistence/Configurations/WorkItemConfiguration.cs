using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Infrastructure.Persistence.Configurations;

public sealed class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems", table => table.HasCheckConstraint(
            "CK_WorkItems_Difficulty",
            $"\"Difficulty\" IS NULL OR \"Difficulty\" BETWEEN {DifficultyLevel.Min} AND {DifficultyLevel.Max}"));

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(2000);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(w => w.TaskType)
            .WithMany(t => t.WorkItems)
            .HasForeignKey(w => w.TaskTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.AssignedEmployee)
            .WithMany(e => e.AssignedWorkItems)
            .HasForeignKey(w => w.AssignedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => new { w.AssignedEmployeeId, w.AssignedAt });
    }
}