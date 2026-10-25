using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.TaskTypes;

internal static class TaskTypeMappings
{
    internal static TaskTypeDto ToDto(this TaskType taskType) => new(
        taskType.Id,
        taskType.Name,
        taskType.Description,
        taskType.Difficulty,
        taskType.CreatedAt);
}