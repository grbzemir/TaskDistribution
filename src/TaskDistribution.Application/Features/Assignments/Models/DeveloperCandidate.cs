namespace TaskDistribution.Application.Features.Assignments.Models;

public sealed record DeveloperCandidate(
    int EmployeeId,
    string FullName,
    int CountAtDifficulty,
    int TotalWorkload,
    int? LastAssignedDifficulty,
    DateTime? LastAssignedAt);