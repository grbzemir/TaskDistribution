using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Application.Abstractions.Persistence;

public interface IWorkItemRepository : IRepository<WorkItem>
{
    Task<IReadOnlyList<WorkItem>> GetAllWithDetailsAsync(
        WorkItemStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssignmentHistoryEntry>> GetAssignmentHistoryAsync(
    CancellationToken cancellationToken = default);

    Task<WorkItem?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}