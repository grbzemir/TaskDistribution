namespace TaskDistribution.Application.Features.Assignments.Settings;

public interface IAssignmentSettingsProvider
{
    Task<AssignmentSettings> GetAsync(CancellationToken cancellationToken = default);
}