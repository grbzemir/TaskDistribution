using TaskDistribution.Application.Features.Assignments.Models;

namespace TaskDistribution.Application.Features.Assignments.Rules;

public sealed class ConsecutiveAssignmentRule : IAssignmentRule
{
    public string Key => AssignmentRuleKeys.ConsecutiveAssignment;

    public string Name => "No back-to-back assignments";

    public string Description =>
        "The developer who received the most recent task cannot receive the next one.";

    public bool IsSatisfiedBy(DeveloperCandidate candidate, AssignmentContext context) =>
        context.LastAssignedEmployeeId is not { } lastEmployeeId
        || candidate.EmployeeId != lastEmployeeId;
}