using TaskDistribution.Application.Features.Assignments.Models;

namespace TaskDistribution.Application.Features.Assignments.Rules;

public sealed class ConsecutiveDifficultyRule : IAssignmentRule
{
    public string Key => AssignmentRuleKeys.ConsecutiveDifficulty;

    public string Name => "No consecutive difficulty levels";

    public string Description =>
        "A developer cannot receive a task whose difficulty is adjacent (±1) to their last assigned task.";

    public bool IsSatisfiedBy(DeveloperCandidate candidate, AssignmentContext context) =>
        candidate.LastAssignedDifficulty is not { } lastDifficulty
        || Math.Abs(lastDifficulty - context.Difficulty) != 1;
}