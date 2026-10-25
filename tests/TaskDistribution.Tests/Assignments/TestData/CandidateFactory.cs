using TaskDistribution.Application.Features.Assignments.Models;

namespace TaskDistribution.Tests.TestData;

internal static class CandidateFactory
{
    internal static readonly DateTime BaseTime = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    internal static DeveloperCandidate Create(
        int employeeId,
        int countAtDifficulty = 0,
        int totalWorkload = 0,
        int? lastDifficulty = null,
        DateTime? lastAssignedAt = null) =>
        new(
            employeeId,
            $"Developer {employeeId}",
            countAtDifficulty,
            totalWorkload,
            lastDifficulty,
            lastAssignedAt);

    internal static AssignmentContext Context(
        int difficulty,
        int? lastAssignedEmployeeId = null,
        int hardTaskThreshold = 6) =>
        new(difficulty, lastAssignedEmployeeId, hardTaskThreshold);
}