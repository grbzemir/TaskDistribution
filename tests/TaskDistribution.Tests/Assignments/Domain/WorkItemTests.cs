using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;
using TaskDistribution.Domain.Exceptions;
using TaskDistribution.Tests.TestData;

namespace TaskDistribution.Tests.Domain;

public sealed class WorkItemTests
{
    private static readonly DateTime Now = CandidateFactory.BaseTime;

    [Fact]
    public void Analyze_SetsTaskTypeDifficultyAndStatus()
    {
        var workItem = CreateWorkItem();

        workItem.Analyze(CreateTaskType(difficulty: 6), Now);

        Assert.Equal(WorkItemStatus.Analyzed, workItem.Status);
        Assert.Equal(6, workItem.Difficulty);
        Assert.Equal(Now, workItem.AnalyzedAt);
    }

    [Fact]
    public void Analyze_WhenTaskTypeDifficultyIsOutOfRange_Throws()
    {
        var workItem = CreateWorkItem();

        Assert.Throws<DomainException>(() => workItem.Analyze(CreateTaskType(difficulty: 9), Now));
    }

    [Fact]
    public void AssignTo_WhenWorkItemIsNotAnalyzed_Throws()
    {
        var workItem = CreateWorkItem();

        Assert.Throws<DomainException>(() => workItem.AssignTo(CreateEmployee(isActive: true), Now));
    }

    [Fact]
    public void AssignTo_WhenEmployeeIsInactive_Throws()
    {
        var workItem = CreateWorkItem();
        workItem.Analyze(CreateTaskType(difficulty: 3), Now);

        Assert.Throws<DomainException>(() => workItem.AssignTo(CreateEmployee(isActive: false), Now));
    }

    [Fact]
    public void Analyze_WhenWorkItemIsAlreadyAssigned_Throws()
    {
        var workItem = CreateWorkItem();
        workItem.Analyze(CreateTaskType(difficulty: 3), Now);
        workItem.AssignTo(CreateEmployee(isActive: true), Now);

        Assert.Throws<DomainException>(() => workItem.Analyze(CreateTaskType(difficulty: 5), Now));
    }

    [Fact]
    public void Complete_WhenWorkItemIsNotAssigned_Throws()
    {
        var workItem = CreateWorkItem();

        Assert.Throws<DomainException>(() => workItem.Complete(Now));
    }

    [Fact]
    public void FullLifecycle_MovesThroughAllStatuses()
    {
        var workItem = CreateWorkItem();
        var employee = CreateEmployee(isActive: true);

        workItem.Analyze(CreateTaskType(difficulty: 4), Now);
        workItem.AssignTo(employee, Now.AddMinutes(5));
        workItem.Complete(Now.AddHours(2));

        Assert.Equal(WorkItemStatus.Completed, workItem.Status);
        Assert.Equal(employee.Id, workItem.AssignedEmployeeId);
        Assert.Equal(Now.AddMinutes(5), workItem.AssignedAt);
        Assert.Equal(Now.AddHours(2), workItem.CompletedAt);
    }

    private static WorkItem CreateWorkItem() => new() { Title = "Fix invoice export" };

    private static TaskType CreateTaskType(int difficulty) => new()
    {
        Id = 1,
        Name = "Test type",
        Difficulty = difficulty
    };

    private static Employee CreateEmployee(bool isActive) => new()
    {
        Id = 1,
        FirstName = "Ayşe",
        LastName = "Kaya",
        IsActive = isActive
    };
}