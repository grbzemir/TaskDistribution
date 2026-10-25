using Microsoft.EntityFrameworkCore;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Infrastructure.Persistence.Repositories;

public sealed class AssignmentLogRepository(TaskDistributionDbContext dbContext)
    : Repository<AssignmentLog>(dbContext), IAssignmentLogRepository
{
    public async Task<IReadOnlyList<AssignmentLog>> GetRecentWithDetailsAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(a => a.WorkItem)
            .Include(a => a.Employee)
            .OrderByDescending(a => a.AssignedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
}