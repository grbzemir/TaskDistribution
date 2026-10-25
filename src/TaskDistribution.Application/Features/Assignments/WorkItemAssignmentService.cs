using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Common.Exceptions;
using TaskDistribution.Application.Features.Assignments.Models;
using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Application.Features.Assignments.Selection;
using TaskDistribution.Application.Features.Assignments.Settings;
using TaskDistribution.Application.Features.WorkItems;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Application.Features.Assignments;

public sealed class WorkItemAssignmentService(
    IWorkItemRepository workItemRepository,
    IEmployeeRepository employeeRepository,
    IAssignmentLogRepository assignmentLogRepository,
    IAssignmentSettingsProvider settingsProvider,
    IEnumerable<IAssignmentRule> rules,
    IDeveloperSelector developerSelector,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IWorkItemAssignmentService
{
    private readonly IReadOnlyList<IAssignmentRule> _rules = rules.ToList();

    public Task<AssignmentResultDto> AssignAsync(
        int workItemId,
        CancellationToken cancellationToken = default) =>
        unitOfWork.ExecuteInTransactionAsync(
            token => AssignInternalAsync(workItemId, token),
            cancellationToken);

    private async Task<AssignmentResultDto> AssignInternalAsync(
        int workItemId,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemRepository.GetByIdWithDetailsAsync(workItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkItem), workItemId);

        if (workItem.Status != WorkItemStatus.Analyzed || workItem.Difficulty is not { } difficulty)
        {
            throw new ConflictException("Only analyzed work items can be assigned.");
        }

        var developers = await employeeRepository.GetActiveByRoleAsync(
            RoleNames.Developer,
            cancellationToken);

        if (developers.Count == 0)
        {
            throw new ConflictException("There are no active developers to assign this work item to.");
        }

        var history = await workItemRepository.GetAssignmentHistoryAsync(cancellationToken);
        var settings = await settingsProvider.GetAsync(cancellationToken);

        var activeRules = _rules
            .Where(rule => settings.EnabledRuleKeys.Contains(rule.Key, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var context = new AssignmentContext(
            difficulty,
            DeveloperCandidateBuilder.FindLastAssignedEmployeeId(history),
            settings.HardTaskThreshold);

        var candidates = DeveloperCandidateBuilder.Build(developers, history, difficulty);
        var selection = developerSelector.Select(candidates, context, activeRules);

        var selectedEmployee = developers.Single(d => d.Id == selection.Selected.EmployeeId);
        var assignedAt = timeProvider.GetUtcNow().UtcDateTime;

        workItem.AssignTo(selectedEmployee, assignedAt);

        await assignmentLogRepository.AddAsync(new AssignmentLog
        {
            WorkItemId = workItem.Id,
            EmployeeId = selectedEmployee.Id,
            Difficulty = difficulty,
            AppliedRules = string.Join(',', selection.AppliedRuleKeys),
            IsRuleRelaxed = selection.IsRuleRelaxed,
            Reason = selection.Reason,
            AssignedAt = assignedAt
        }, cancellationToken);

        return new AssignmentResultDto(
            workItem.ToDto(),
            selectedEmployee.FullName,
            selection.AppliedRuleKeys,
            selection.IsRuleRelaxed,
            selection.Reason);
    }
}