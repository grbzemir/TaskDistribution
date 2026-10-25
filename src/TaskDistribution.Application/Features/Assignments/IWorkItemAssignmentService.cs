namespace TaskDistribution.Application.Features.Assignments;

public interface IWorkItemAssignmentService
{
    Task<AssignmentResultDto> AssignAsync(int workItemId, CancellationToken cancellationToken = default);
}