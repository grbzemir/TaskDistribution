using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Tests.TestData;

namespace TaskDistribution.Tests.Assignments.Rules;

public sealed class ConsecutiveAssignmentRuleTests
{
    private readonly ConsecutiveAssignmentRule _rule = new();

    [Fact]
    public void IsSatisfiedBy_WhenDeveloperReceivedTheLastTask_ReturnsFalse()
    {
        var candidate = CandidateFactory.Create(3);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(5, lastAssignedEmployeeId: 3));

        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenAnotherDeveloperReceivedTheLastTask_ReturnsTrue()
    {
        var candidate = CandidateFactory.Create(3);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(5, lastAssignedEmployeeId: 7));

        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WhenNoTaskHasBeenAssignedYet_ReturnsTrue()
    {
        var candidate = CandidateFactory.Create(3);

        var result = _rule.IsSatisfiedBy(candidate, CandidateFactory.Context(5, lastAssignedEmployeeId: null));

        Assert.True(result);
    }
}