namespace TaskDistribution.Application.Features.WorkItems;

public sealed record WorkItemDto(
    int Id,
    string Title,
    string? Description,
    string Status,
    int? TaskTypeId,
    string? TaskTypeName,
    int? Difficulty,
    int? AssignedEmployeeId,
    string? AssignedEmployeeName,
    DateTime CreatedAt,
    DateTime? AnalyzedAt,
    DateTime? AssignedAt,
    DateTime? CompletedAt);

public sealed record WorkItemRequest(string Title, string? Description);

public sealed record AnalyzeWorkItemRequest(int TaskTypeId);