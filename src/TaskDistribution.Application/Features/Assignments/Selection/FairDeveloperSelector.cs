using TaskDistribution.Application.Features.Assignments.Models;
using TaskDistribution.Application.Features.Assignments.Rules;

namespace TaskDistribution.Application.Features.Assignments.Selection;

public sealed class FairDeveloperSelector : IDeveloperSelector
{
    public SelectionResult Select(
        IReadOnlyCollection<DeveloperCandidate> candidates,
        AssignmentContext context,
        IReadOnlyCollection<IAssignmentRule> rules)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(rules);

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException("At least one developer candidate is required.");
        }

        var eligibleCandidates = candidates
            .Where(candidate => rules.All(rule => rule.IsSatisfiedBy(candidate, context)))
            .ToList();

        var isRuleRelaxed = eligibleCandidates.Count == 0;
        var pool = isRuleRelaxed ? candidates : eligibleCandidates;

        var selected = OrderByFairness(pool).First();
        var appliedRuleKeys = rules.Select(rule => rule.Key).ToList();

        var reason = BuildReason(
            selected,
            context,
            isRuleRelaxed,
            eligibleCandidates.Count,
            candidates.Count);

        return new SelectionResult(selected, appliedRuleKeys, isRuleRelaxed, reason);
    }

    public static IOrderedEnumerable<DeveloperCandidate> OrderByFairness(
        IEnumerable<DeveloperCandidate> candidates) =>
        candidates
            .OrderBy(candidate => candidate.CountAtDifficulty)
            .ThenBy(candidate => candidate.TotalWorkload)
            .ThenBy(candidate => candidate.LastAssignedAt ?? DateTime.MinValue)
            .ThenBy(candidate => candidate.EmployeeId);

    private static string BuildReason(
        DeveloperCandidate selected,
        AssignmentContext context,
        bool isRuleRelaxed,
        int eligibleCount,
        int totalCount)
    {
        var lastAssignment = selected.LastAssignedAt is { } lastAssignedAt
            ? $"last assigned at {lastAssignedAt:yyyy-MM-dd HH:mm} UTC"
            : "no previous assignments";

        var fairnessSummary =
            $"{selected.CountAtDifficulty} task(s) at level {context.Difficulty}, " +
            $"total workload {selected.TotalWorkload}, {lastAssignment}";

        return isRuleRelaxed
            ? $"No developer satisfied all active rules, so the rules were relaxed. " +
              $"{selected.FullName} was selected as the fairest choice ({fairnessSummary})."
            : $"{selected.FullName} was selected among {eligibleCount} of {totalCount} " +
              $"eligible developers ({fairnessSummary}).";
    }
}