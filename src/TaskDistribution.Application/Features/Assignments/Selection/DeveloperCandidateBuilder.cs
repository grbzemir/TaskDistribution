using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Features.Assignments.Models;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.Assignments.Selection;
public static class DeveloperCandidateBuilder
{
    public static IReadOnlyList<DeveloperCandidate> Build(
        IEnumerable<Employee> developers,
        IReadOnlyCollection<AssignmentHistoryEntry> history,
        int difficulty)
    {
        ArgumentNullException.ThrowIfNull(developers);
        ArgumentNullException.ThrowIfNull(history);

        var historyByEmployee = history
            .GroupBy(entry => entry.EmployeeId)
            .ToDictionary(group => group.Key, group => group.ToList());

        return developers
            .Select(developer => historyByEmployee.TryGetValue(developer.Id, out var entries)
                ? CreateCandidate(developer, entries, difficulty)
                : new DeveloperCandidate(developer.Id, developer.FullName, 0, 0, null, null))
            .ToList();
    }

    public static int? FindLastAssignedEmployeeId(IReadOnlyCollection<AssignmentHistoryEntry> history) =>
        history
            .OrderByDescending(entry => entry.AssignedAt)
            .ThenByDescending(entry => entry.WorkItemId)
            .Select(entry => (int?)entry.EmployeeId)
            .FirstOrDefault();

    private static DeveloperCandidate CreateCandidate(
        Employee developer,
        List<AssignmentHistoryEntry> entries,
        int difficulty)
    {
        var lastEntry = entries
            .OrderByDescending(entry => entry.AssignedAt)
            .ThenByDescending(entry => entry.WorkItemId)
            .First();

        return new DeveloperCandidate(
            developer.Id,
            developer.FullName,
            CountAtDifficulty: entries.Count(entry => entry.Difficulty == difficulty),
            TotalWorkload: entries.Sum(entry => entry.Difficulty),
            LastAssignedDifficulty: lastEntry.Difficulty,
            LastAssignedAt: lastEntry.AssignedAt);
    }
}