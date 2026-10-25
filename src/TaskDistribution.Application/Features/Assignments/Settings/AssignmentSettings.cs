namespace TaskDistribution.Application.Features.Assignments.Settings;

public sealed record AssignmentSettings(
    IReadOnlyCollection<string> EnabledRuleKeys,
    int HardTaskThreshold);