namespace TaskDistribution.Application.Features.Dashboard;

public sealed record DeveloperWorkloadDto(
    int EmployeeId,
    string FullName,
    bool IsActive,
    int TaskCount,
    int OpenTaskCount,
    int TotalWorkload,
    IReadOnlyList<int> CountsByDifficulty,
    DateTime? LastAssignedAt);

public sealed record DashboardSummaryDto(
    int TotalWorkItems,
    int NewCount,
    int AnalyzedCount,
    int AssignedCount,
    int CompletedCount,
    int ActiveDeveloperCount,
    double FairnessIndex,
    int MaxSpreadPerDifficulty,
    IReadOnlyList<DeveloperWorkloadDto> Developers);