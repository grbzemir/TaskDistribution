using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Application.Features.WorkItems;

public interface IWorkItemService
{
    Task<IReadOnlyList<WorkItemDto>> GetAllAsync(
        WorkItemStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<WorkItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<WorkItemDto> CreateAsync(WorkItemRequest request, CancellationToken cancellationToken = default);

    Task<WorkItemDto> UpdateAsync(int id, WorkItemRequest request, CancellationToken cancellationToken = default);

    Task<WorkItemDto> AnalyzeAsync(
        int id,
        AnalyzeWorkItemRequest request,
        CancellationToken cancellationToken = default);

    Task<WorkItemDto> CompleteAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}