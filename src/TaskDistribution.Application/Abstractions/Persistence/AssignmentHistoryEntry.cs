namespace TaskDistribution.Application.Abstractions.Persistence;

public sealed record AssignmentHistoryEntry(
    int WorkItemId,
    int EmployeeId,
    int Difficulty,
    DateTime AssignedAt);