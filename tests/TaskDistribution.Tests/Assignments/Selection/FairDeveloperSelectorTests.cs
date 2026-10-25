using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Application.Features.Assignments.Selection;
using TaskDistribution.Tests.TestData;

namespace TaskDistribution.Tests.Assignments.Selection;

public sealed class FairDeveloperSelectorTests
{
    private readonly FairDeveloperSelector _selector = new();

    [Fact]
    public void Select_PrefersDeveloperWithFewestTasksAtSameDifficulty()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, countAtDifficulty: 2, totalWorkload: 5),
                CandidateFactory.Create(2, countAtDifficulty: 1, totalWorkload: 30)
            ],
            CandidateFactory.Context(5),
            []);

        Assert.Equal(2, result.Selected.EmployeeId);
    }

    [Fact]
    public void Select_WhenCountsAreEqual_PrefersLowerTotalWorkload()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, countAtDifficulty: 1, totalWorkload: 20),
                CandidateFactory.Create(2, countAtDifficulty: 1, totalWorkload: 12)
            ],
            CandidateFactory.Context(5),
            []);

        Assert.Equal(2, result.Selected.EmployeeId);
    }

    [Fact]
    public void Select_WhenWorkloadsAreEqual_PrefersDeveloperIdleForLongest()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, 1, 10, lastDifficulty: 2, lastAssignedAt: CandidateFactory.BaseTime.AddHours(3)),
                CandidateFactory.Create(2, 1, 10, lastDifficulty: 2, lastAssignedAt: CandidateFactory.BaseTime.AddHours(1)),
                CandidateFactory.Create(3, 1, 10, lastDifficulty: 2, lastAssignedAt: CandidateFactory.BaseTime.AddHours(2))
            ],
            CandidateFactory.Context(5),
            []);

        Assert.Equal(2, result.Selected.EmployeeId);
    }

    [Fact]
    public void Select_WhenWorkloadsAreEqual_PrefersDeveloperWithoutPreviousAssignments()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, lastDifficulty: 2, lastAssignedAt: CandidateFactory.BaseTime),
                CandidateFactory.Create(2)
            ],
            CandidateFactory.Context(5),
            []);

        Assert.Equal(2, result.Selected.EmployeeId);
    }

    [Fact]
    public void Select_WhenAllCriteriaAreEqual_PrefersLowestEmployeeId()
    {
        var result = _selector.Select(
            [CandidateFactory.Create(7), CandidateFactory.Create(3), CandidateFactory.Create(5)],
            CandidateFactory.Context(5),
            []);

        Assert.Equal(3, result.Selected.EmployeeId);
    }

    [Fact]
    public void Select_ExcludesCandidatesThatViolateActiveRules()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, lastDifficulty: 4, lastAssignedAt: CandidateFactory.BaseTime),
                CandidateFactory.Create(2, countAtDifficulty: 3, totalWorkload: 40, lastDifficulty: 2)
            ],
            CandidateFactory.Context(5),
            [new ConsecutiveDifficultyRule()]);

        Assert.Equal(2, result.Selected.EmployeeId);
        Assert.False(result.IsRuleRelaxed);
    }

    [Fact]
    public void Select_WhenNoCandidateSatisfiesRules_RelaxesRulesAndSelectsFairest()
    {
        var result = _selector.Select(
            [
                CandidateFactory.Create(1, countAtDifficulty: 1, totalWorkload: 9, lastDifficulty: 4),
                CandidateFactory.Create(2, countAtDifficulty: 0, totalWorkload: 6, lastDifficulty: 6)
            ],
            CandidateFactory.Context(5),
            [new ConsecutiveDifficultyRule()]);

        Assert.True(result.IsRuleRelaxed);
        Assert.Equal(2, result.Selected.EmployeeId);
        Assert.Contains("relaxed", result.Reason);
    }

    [Fact]
    public void Select_ReturnsKeysOfAppliedRules()
    {
        var result = _selector.Select(
            [CandidateFactory.Create(1)],
            CandidateFactory.Context(5),
            [new ConsecutiveDifficultyRule(), new ConsecutiveHardTaskRule()]);

        Assert.Equal(
            [AssignmentRuleKeys.ConsecutiveDifficulty, AssignmentRuleKeys.ConsecutiveHardTask],
            result.AppliedRuleKeys);
    }

    [Fact]
    public void Select_ReasonContainsSelectedDeveloperName()
    {
        var result = _selector.Select(
            [CandidateFactory.Create(4)],
            CandidateFactory.Context(5),
            []);

        Assert.Contains("Developer 4", result.Reason);
    }

    [Fact]
    public void Select_WhenNoCandidates_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _selector.Select([], CandidateFactory.Context(5), []));
    }
}