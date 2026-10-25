using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Application.Features.Dashboard;

public sealed class DashboardService(
    IEmployeeRepository employeeRepository,
    IWorkItemRepository workItemRepository) : IDashboardService
{
    private static readonly IReadOnlyList<int> DifficultyLevels = Enumerable
        .Range(DifficultyLevel.Min, DifficultyLevel.Max - DifficultyLevel.Min + 1)
        .ToList();

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllWithRoleAsync(cancellationToken);
        var workItems = await workItemRepository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        var itemsByEmployee = workItems
            .Where(w => w.AssignedEmployeeId.HasValue && w.Difficulty.HasValue)
            .GroupBy(w => w.AssignedEmployeeId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var developers = employees
            .Where(e => e.Role.Name == RoleNames.Developer && (e.IsActive || itemsByEmployee.ContainsKey(e.Id)))
            .Select(e => BuildWorkload(e, itemsByEmployee.GetValueOrDefault(e.Id) ?? []))
            .ToList();

        var activeDevelopers = developers.Where(d => d.IsActive).ToList();

        var fairnessIndex = FairnessCalculator.CalculateJainIndex(
            activeDevelopers.Select(d => d.TotalWorkload).ToList());

        var maxSpread = FairnessCalculator.CalculateMaxSpreadPerLevel(
            activeDevelopers.Select(d => d.CountsByDifficulty).ToList());

        return new DashboardSummaryDto(
            TotalWorkItems: workItems.Count,
            NewCount: workItems.Count(w => w.Status == WorkItemStatus.New),
            AnalyzedCount: workItems.Count(w => w.Status == WorkItemStatus.Analyzed),
            AssignedCount: workItems.Count(w => w.Status == WorkItemStatus.Assigned),
            CompletedCount: workItems.Count(w => w.Status == WorkItemStatus.Completed),
            ActiveDeveloperCount: activeDevelopers.Count,
            FairnessIndex: Math.Round(fairnessIndex, 3),
            MaxSpreadPerDifficulty: maxSpread,
            Developers: developers);
    }

    private static DeveloperWorkloadDto BuildWorkload(Employee employee, List<WorkItem> items) => new(
        employee.Id,
        employee.FullName,
        employee.IsActive,
        TaskCount: items.Count,
        OpenTaskCount: items.Count(w => w.Status == WorkItemStatus.Assigned),
        TotalWorkload: items.Sum(w => w.Difficulty!.Value),
        CountsByDifficulty: DifficultyLevels.Select(level => items.Count(w => w.Difficulty == level)).ToList(),
        LastAssignedAt: items.Max(w => w.AssignedAt));
}