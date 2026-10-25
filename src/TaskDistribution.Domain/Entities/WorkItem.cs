using TaskDistribution.Domain.Common;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Enums;
using TaskDistribution.Domain.Exceptions;

namespace TaskDistribution.Domain.Entities;

public class WorkItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public WorkItemStatus Status { get; private set; } = WorkItemStatus.New;

    public int? TaskTypeId { get; private set; }

    public TaskType? TaskType { get; private set; }

    public int? Difficulty { get; private set; }

    public DateTime? AnalyzedAt { get; private set; }

    public int? AssignedEmployeeId { get; private set; }

    public Employee? AssignedEmployee { get; private set; }

    public DateTime? AssignedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public void Analyze(TaskType taskType, DateTime analyzedAt)
    {
        ArgumentNullException.ThrowIfNull(taskType);

        if (Status is not (WorkItemStatus.New or WorkItemStatus.Analyzed))
        {
            throw new DomainException("Only new or analyzed work items can be analyzed.");
        }

        DifficultyLevel.EnsureValid(taskType.Difficulty);

        TaskTypeId = taskType.Id;
        TaskType = taskType;
        Difficulty = taskType.Difficulty;
        AnalyzedAt = analyzedAt;
        Status = WorkItemStatus.Analyzed;
    }

    public void AssignTo(Employee employee, DateTime assignedAt)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (Status != WorkItemStatus.Analyzed)
        {
            throw new DomainException("Only analyzed work items can be assigned.");
        }

        if (!employee.IsActive)
        {
            throw new DomainException("Work items cannot be assigned to inactive employees.");
        }

        AssignedEmployeeId = employee.Id;
        AssignedEmployee = employee;
        AssignedAt = assignedAt;
        Status = WorkItemStatus.Assigned;
    }

    public void Complete(DateTime completedAt)
    {
        if (Status != WorkItemStatus.Assigned)
        {
            throw new DomainException("Only assigned work items can be completed.");
        }

        CompletedAt = completedAt;
        Status = WorkItemStatus.Completed;
    }
}