using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Features.Assignments.Selection;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Tests.TestData;


namespace TaskDistribution.Tests.Assignments.Selection;

public sealed class DeveloperCandidateBuilderTests
{
    private static readonly DateTime T0 = CandidateFactory.BaseTime;

    [Fact]
    public void Build_WhenDeveloperHasNoHistory_ReturnsEmptyStatistics()
    {
        var candidates = DeveloperCandidateBuilder.Build([CreateDeveloper(1)], [], difficulty: 5);

        var candidate = Assert.Single(candidates);
        Assert.Equal(0, candidate.CountAtDifficulty);
        Assert.Equal(0, candidate.TotalWorkload);
        Assert.Null(candidate.LastAssignedDifficulty);
        Assert.Null(candidate.LastAssignedAt);
    }

    [Fact]
    public void Build_CalculatesStatisticsFromHistory()
    {
        AssignmentHistoryEntry[] history =
        [
            new(1, EmployeeId: 1, Difficulty: 5, AssignedAt: T0),
            new(2, EmployeeId: 1, Difficulty: 3, AssignedAt: T0.AddMinutes(10)),
            new(3, EmployeeId: 1, Difficulty: 5, AssignedAt: T0.AddMinutes(20)),
            new(4, EmployeeId: 2, Difficulty: 8, AssignedAt: T0.AddMinutes(30))
        ];

        var candidates = DeveloperCandidateBuilder.Build([CreateDeveloper(1)], history, difficulty: 5);

        var candidate = Assert.Single(candidates);
        Assert.Equal(2, candidate.CountAtDifficulty);
        Assert.Equal(13, candidate.TotalWorkload);
        Assert.Equal(5, candidate.LastAssignedDifficulty);
        Assert.Equal(T0.AddMinutes(20), candidate.LastAssignedAt);
    }

    [Fact]
    public void Build_WhenAssignmentsShareTimestamp_UsesHigherWorkItemIdAsLast()
    {
        AssignmentHistoryEntry[] history =
        [
            new(10, EmployeeId: 1, Difficulty: 2, AssignedAt: T0),
            new(11, EmployeeId: 1, Difficulty: 7, AssignedAt: T0)
        ];

        var candidate = Assert.Single(DeveloperCandidateBuilder.Build([CreateDeveloper(1)], history, 5));

        Assert.Equal(7, candidate.LastAssignedDifficulty);
    }

    [Fact]
    public void Build_ReturnsOneCandidatePerDeveloper()
    {
        var candidates = DeveloperCandidateBuilder.Build(
            [CreateDeveloper(1), CreateDeveloper(2), CreateDeveloper(3)],
            [new(1, EmployeeId: 2, Difficulty: 4, AssignedAt: T0)],
            difficulty: 4);

        Assert.Equal([1, 2, 3], candidates.Select(c => c.EmployeeId));
        Assert.Equal(1, candidates.Single(c => c.EmployeeId == 2).CountAtDifficulty);
    }

    [Fact]
    public void FindLastAssignedEmployeeId_WhenHistoryIsEmpty_ReturnsNull()
    {
        Assert.Null(DeveloperCandidateBuilder.FindLastAssignedEmployeeId([]));
    }

    [Fact]
    public void FindLastAssignedEmployeeId_ReturnsEmployeeOfMostRecentAssignment()
    {
        AssignmentHistoryEntry[] history =
        [
            new(1, EmployeeId: 4, Difficulty: 2, AssignedAt: T0.AddMinutes(5)),
            new(2, EmployeeId: 6, Difficulty: 3, AssignedAt: T0.AddMinutes(15)),
            new(3, EmployeeId: 2, Difficulty: 1, AssignedAt: T0)
        ];

        Assert.Equal(6, DeveloperCandidateBuilder.FindLastAssignedEmployeeId(history));
    }

    private static Employee CreateDeveloper(int id) => new()
    {
        Id = id,
        FirstName = "Developer",
        LastName = id.ToString(),
        IsActive = true
    };
}