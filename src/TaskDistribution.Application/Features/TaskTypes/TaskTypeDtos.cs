namespace TaskDistribution.Application.Features.TaskTypes;
public sealed record TaskTypeDto(
    int Id,
    string Name,
    string? Description,
    int Difficulty,
    DateTime CreatedAt);

public sealed record TaskTypeRequest(string Name, string? Description, int Difficulty);