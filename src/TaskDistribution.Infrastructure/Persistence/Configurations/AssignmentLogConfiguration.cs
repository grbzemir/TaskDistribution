using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Infrastructure.Persistence.Configurations;
    public sealed class AssignmentLogConfiguration : IEntityTypeConfiguration<AssignmentLog>
    {
        public void Configure(EntityTypeBuilder<AssignmentLog> builder)
        {
            builder.ToTable("AssignmentLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.AppliedRules)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(a => a.WorkItem)
                .WithMany()
                .HasForeignKey(a => a.WorkItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.EmployeeId, a.AssignedAt });
        }
    }