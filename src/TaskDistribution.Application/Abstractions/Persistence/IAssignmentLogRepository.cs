using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Abstractions.Persistence;
public interface IAssignmentLogRepository : IRepository<AssignmentLog>
{
    Task<IReadOnlyList<AssignmentLog>> GetRecentWithDetailsAsync(
        int count,
        CancellationToken cancellationToken = default);
}