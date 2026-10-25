using TaskDistribution.Application.Features.WorkItems;

namespace TaskDistribution.Application.Features.Assignments;

public sealed record AssignmentResultDto(
    WorkItemDto WorkItem,
    string AssignedEmployeeName,
    IReadOnlyList<string> AppliedRules,
    bool IsRuleRelaxed,
    string Reason);

public sealed record AssignmentRuleDto(
    string Key,
    string Name,
    string Description,
    bool IsEnabled);

public sealed record AssignmentLogDto(
    int Id,
    int WorkItemId,
    string WorkItemTitle,
    int EmployeeId,
    string EmployeeName,
    int Difficulty,
    IReadOnlyList<string> AppliedRules,
    bool IsRuleRelaxed,
    string Reason,
    DateTime AssignedAt);