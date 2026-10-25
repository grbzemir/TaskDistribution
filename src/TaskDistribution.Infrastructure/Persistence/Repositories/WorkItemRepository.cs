using Microsoft.EntityFrameworkCore;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Infrastructure.Persistence.Repositories;

public sealed class WorkItemRepository(TaskDistributionDbContext dbContext)
    : Repository<WorkItem>(dbContext), IWorkItemRepository
{
    public async Task<IReadOnlyList<WorkItem>> GetAllWithDetailsAsync(
        WorkItemStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(w => w.TaskType)
            .Include(w => w.AssignedEmployee)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(w => w.Status == status.Value);
        }

        return await query
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssignmentHistoryEntry>> GetAssignmentHistoryAsync(
    CancellationToken cancellationToken = default) =>
    await DbSet
        .AsNoTracking()
        .Where(w => w.AssignedEmployeeId != null && w.Difficulty != null && w.AssignedAt != null)
        .Select(w => new AssignmentHistoryEntry(
            w.Id,
            w.AssignedEmployeeId!.Value,
            w.Difficulty!.Value,
            w.AssignedAt!.Value))
        .ToListAsync(cancellationToken);

    public Task<WorkItem?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(w => w.TaskType)
            .Include(w => w.AssignedEmployee)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
}