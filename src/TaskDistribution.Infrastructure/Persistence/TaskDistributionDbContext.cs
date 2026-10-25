using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Infrastructure.Persistence.Converters;

namespace TaskDistribution.Infrastructure.Persistence;

public sealed class TaskDistributionDbContext(DbContextOptions<TaskDistributionDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<TaskType> TaskTypes => Set<TaskType>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<AssignmentLog> AssignmentLogs => Set<AssignmentLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDistributionDbContext).Assembly);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcDateTimeConverter>();
    }
}
