using TaskDistribution.Application.Features.Assignments.Models;
using TaskDistribution.Application.Features.Assignments.Rules;

namespace TaskDistribution.Application.Features.Assignments.Selection;

public interface IDeveloperSelector
{
    SelectionResult Select(
        IReadOnlyCollection<DeveloperCandidate> candidates,
        AssignmentContext context,
        IReadOnlyCollection<IAssignmentRule> rules);
}