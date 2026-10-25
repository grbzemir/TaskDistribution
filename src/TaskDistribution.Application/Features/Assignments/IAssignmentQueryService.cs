namespace TaskDistribution.Application.Features.Assignments;

public interface IAssignmentQueryService
{
    Task<IReadOnlyList<AssignmentRuleDto>> GetRulesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssignmentLogDto>> GetRecentLogsAsync(
        int count,
        CancellationToken cancellationToken = default);
}