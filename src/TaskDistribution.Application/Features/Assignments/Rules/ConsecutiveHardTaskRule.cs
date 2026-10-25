using TaskDistribution.Application.Features.Assignments.Models;

namespace TaskDistribution.Application.Features.Assignments.Rules;

public sealed class ConsecutiveHardTaskRule : IAssignmentRule
{
    public string Key => AssignmentRuleKeys.ConsecutiveHardTask;

    public string Name => "No consecutive hard tasks";

    public string Description =>
        "A developer whose last task was hard (difficulty at or above the configured threshold) cannot receive another hard task.";

    public bool IsSatisfiedBy(DeveloperCandidate candidate, AssignmentContext context)
    {
        var isHardTask = context.Difficulty >= context.HardTaskThreshold;

        if (!isHardTask || candidate.LastAssignedDifficulty is not { } lastDifficulty)
        {
            return true;
        }

        return lastDifficulty < context.HardTaskThreshold;
    }
}