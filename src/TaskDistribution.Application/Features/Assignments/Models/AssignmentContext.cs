namespace TaskDistribution.Application.Features.Assignments.Models;

public sealed record AssignmentContext(
    int Difficulty,
    int? LastAssignedEmployeeId,
    int HardTaskThreshold);