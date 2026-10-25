using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Application.Features.Assignments.Settings;

namespace TaskDistribution.Application.Features.Assignments;

public sealed class AssignmentQueryService(
    IAssignmentLogRepository assignmentLogRepository,
    IAssignmentSettingsProvider settingsProvider,
    IEnumerable<IAssignmentRule> rules) : IAssignmentQueryService
{
    private const int MaxLogCount = 200;

    public async Task<IReadOnlyList<AssignmentRuleDto>> GetRulesAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await settingsProvider.GetAsync(cancellationToken);

        return rules
            .Select(rule => new AssignmentRuleDto(
                rule.Key,
                rule.Name,
                rule.Description,
                settings.EnabledRuleKeys.Contains(rule.Key, StringComparer.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<IReadOnlyList<AssignmentLogDto>> GetRecentLogsAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        var logs = await assignmentLogRepository.GetRecentWithDetailsAsync(
            Math.Clamp(count, 1, MaxLogCount),
            cancellationToken);

        return logs
            .Select(log => new AssignmentLogDto(
                log.Id,
                log.WorkItemId,
                log.WorkItem?.Title ?? string.Empty,
                log.EmployeeId,
                log.Employee?.FullName ?? string.Empty,
                log.Difficulty,
                log.AppliedRules.Split(',', StringSplitOptions.RemoveEmptyEntries),
                log.IsRuleRelaxed,
                log.Reason,
                log.AssignedAt))
            .ToList();
    }
}