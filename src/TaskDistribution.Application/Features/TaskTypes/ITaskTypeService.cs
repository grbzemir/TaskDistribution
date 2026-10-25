namespace TaskDistribution.Application.Features.TaskTypes;

public interface ITaskTypeService
{
    Task<IReadOnlyList<TaskTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskTypeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskTypeDto> CreateAsync(TaskTypeRequest request, CancellationToken cancellationToken = default);
    Task<TaskTypeDto> UpdateAsync(int id, TaskTypeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}