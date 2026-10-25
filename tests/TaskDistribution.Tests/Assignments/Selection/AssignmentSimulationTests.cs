using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Features.Assignments.Models;
using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Application.Features.Assignments.Selection;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Tests.TestData;


namespace TaskDistribution.Tests.Assignments.Selection;

public sealed class AssignmentSimulationTests
{
    private const int DeveloperCount = 8;
    private const int TaskCount = 160;
    private const int HardTaskThreshold = 6;

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(2026)]
    public void DefaultRule_DistributesTasksFairly_AndNeverAssignsConsecutiveDifficulty(int seed)
    {
        var result = Simulate(seed, [new ConsecutiveDifficultyRule()]);

        Assert.Equal(0, result.RuleViolations);
        Assert.True(result.MaxSpreadPerDifficulty <= 2,
            $"Per-difficulty spread was {result.MaxSpreadPerDifficulty}.");
        Assert.True(result.TotalCountSpread <= 3,
            $"Total task count spread was {result.TotalCountSpread}.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(2026)]
    public void AllRules_KeepDistributionFair_AndRespectEveryActiveRule(int seed)
    {
        var result = Simulate(
            seed,
            [new ConsecutiveDifficultyRule(), new ConsecutiveAssignmentRule(), new ConsecutiveHardTaskRule()]);

        Assert.Equal(0, result.RuleViolations);
        Assert.True(result.MaxSpreadPerDifficulty <= 2,
            $"Per-difficulty spread was {result.MaxSpreadPerDifficulty}.");
        Assert.True(result.TotalCountSpread <= 3,
            $"Total task count spread was {result.TotalCountSpread}.");
    }

    private static SimulationResult Simulate(int seed, IReadOnlyCollection<IAssignmentRule> rules)
    {
        var selector = new FairDeveloperSelector();
        var sequence = new DeterministicDifficultySequence(seed);
        var developers = Enumerable.Range(1, DeveloperCount)
            .Select(id => new Employee { Id = id, FirstName = "Developer", LastName = id.ToString(), IsActive = true })
            .ToList();

        var history = new List<AssignmentHistoryEntry>();
        var ruleViolations = 0;

        for (var i = 0; i < TaskCount; i++)
        {
            var difficulty = sequence.Next();

            var context = new AssignmentContext(
                difficulty,
                DeveloperCandidateBuilder.FindLastAssignedEmployeeId(history),
                HardTaskThreshold);

            var candidates = DeveloperCandidateBuilder.Build(developers, history, difficulty);
            var selection = selector.Select(candidates, context, rules);

            if (!selection.IsRuleRelaxed && rules.Any(rule => !rule.IsSatisfiedBy(selection.Selected, context)))
            {
                ruleViolations++;
            }

            history.Add(new AssignmentHistoryEntry(
                WorkItemId: i + 1,
                EmployeeId: selection.Selected.EmployeeId,
                Difficulty: difficulty,
                AssignedAt: CandidateFactory.BaseTime.AddMinutes(i)));
        }

        return new SimulationResult(
            ruleViolations,
            CalculateMaxSpreadPerDifficulty(history, developers),
            CalculateSpread(developers.Select(d => history.Count(h => h.EmployeeId == d.Id))));
    }

    private static int CalculateMaxSpreadPerDifficulty(
        IReadOnlyCollection<AssignmentHistoryEntry> history,
        IReadOnlyCollection<Employee> developers) =>
        Enumerable.Range(DifficultyLevel.Min, DifficultyLevel.Max)
            .Select(level => CalculateSpread(
                developers.Select(d => history.Count(h => h.EmployeeId == d.Id && h.Difficulty == level))))
            .Max();

    private static int CalculateSpread(IEnumerable<int> values)
    {
        var list = values.ToList();
        return list.Max() - list.Min();
    }

    private sealed record SimulationResult(
        int RuleViolations,
        int MaxSpreadPerDifficulty,
        int TotalCountSpread);

    private sealed class DeterministicDifficultySequence(int seed)
    {
        private long _state = seed;

        public int Next()
        {
            _state = (_state * 1103515245 + 12345) % 2147483648;
            return (int)((_state >> 16) % DifficultyLevel.Max) + DifficultyLevel.Min;
        }
    }
}