using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.WorkItems;

internal static class WorkItemMappings
{
    internal static WorkItemDto ToDto(this WorkItem workItem) => new(
        workItem.Id,
        workItem.Title,
        workItem.Description,
        workItem.Status.ToString(),
        workItem.TaskTypeId,
        workItem.TaskType?.Name,
        workItem.Difficulty,
        workItem.AssignedEmployeeId,
        workItem.AssignedEmployee?.FullName,
        workItem.CreatedAt,
        workItem.AnalyzedAt,
        workItem.AssignedAt,
        workItem.CompletedAt);
}