namespace TaskDistribution.Application.Features.Assignments.Models;

public sealed record SelectionResult(
    DeveloperCandidate Selected,
    IReadOnlyList<string> AppliedRuleKeys,
    bool IsRuleRelaxed,
    string Reason);