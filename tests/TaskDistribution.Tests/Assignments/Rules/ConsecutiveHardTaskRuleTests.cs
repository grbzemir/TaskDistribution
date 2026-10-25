using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Tests.TestData;

namespace TaskDistribution.Tests.Assignments.Rules;

public sealed class ConsecutiveHardTaskRuleTests
{
    private readonly ConsecutiveHardTaskRule _rule = new();

    [Theory]
    [InlineData(7, 8)]
    [InlineData(6, 6)]
    [InlineData(8, 7)]
    public void IsSatisfiedBy_WhenLastAndNewTasksAreHard_ReturnsFalse(int lastDifficulty, int newDifficulty)
    {
        var candidate = CandidateFactory.Create(1, lastDifficulty: lastDifficulty);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(newDifficulty));

        Assert.False(result);
    }

    [Theory]
    [InlineData(7, 3)]
    [InlineData(3, 8)]
    [InlineData(5, 6)]
    public void IsSatisfiedBy_WhenEitherTaskIsNotHard_ReturnsTrue(int lastDifficulty, int newDifficulty)
    {
        var candidate = CandidateFactory.Create(1, lastDifficulty: lastDifficulty);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(newDifficulty));

        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenDeveloperHasNoPreviousTask_ReturnsTrue()
    {
        var candidate = CandidateFactory.Create(1);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(8));

        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_UsesConfiguredThreshold()
    {
        var candidate = CandidateFactory.Create(1, lastDifficulty: 5);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(4, hardTaskThreshold: 4));

        Assert.False(result);
    }
}