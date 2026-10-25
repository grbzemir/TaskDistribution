using TaskDistribution.Application.Features.Assignments.Models;

namespace TaskDistribution.Application.Features.Assignments.Rules;
public interface IAssignmentRule
{
    string Key { get; }

    string Name { get; }

    string Description { get; }

    bool IsSatisfiedBy(DeveloperCandidate candidate, AssignmentContext context);
}