using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Tests.TestData;

namespace TaskDistribution.Tests.Assignments.Rules;

public sealed class ConsecutiveDifficultyRuleTests
{
    private readonly ConsecutiveDifficultyRule _rule = new();

    [Theory]
    [InlineData(5, 4)]
    [InlineData(5, 6)]
    [InlineData(1, 2)]
    [InlineData(8, 7)]
    public void IsSatisfiedBy_WhenDifficultyIsAdjacentToLastTask_ReturnsFalse(int lastDifficulty, int newDifficulty)
    {
        var candidate = CandidateFactory.Create(1, lastDifficulty: lastDifficulty);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(newDifficulty));

        Assert.False(result);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, 3)]
    [InlineData(5, 7)]
    [InlineData(1, 8)]
    public void IsSatisfiedBy_WhenDifficultyIsNotAdjacentToLastTask_ReturnsTrue(int lastDifficulty, int newDifficulty)
    {
        var candidate = CandidateFactory.Create(1, lastDifficulty: lastDifficulty);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(newDifficulty));

        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenDeveloperHasNoPreviousTask_ReturnsTrue()
    {
        var candidate = CandidateFactory.Create(1);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(5));

        Assert.True(result);
    }

    [Fact]
    public void Key_MatchesConfigurationKey()
    {
        Assert.Equal(AssignmentRuleKeys.ConsecutiveDifficulty, _rule.Key);
    }
}