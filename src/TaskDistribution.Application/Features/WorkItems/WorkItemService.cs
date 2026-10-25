using FluentValidation;

using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Common.Exceptions;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.Application.Features.WorkItems;

public sealed class WorkItemService(
    IWorkItemRepository workItemRepository,
    IRepository<TaskType> taskTypeRepository,
    IUnitOfWork unitOfWork,
    IValidator<WorkItemRequest> requestValidator,
    IValidator<AnalyzeWorkItemRequest> analyzeValidator,
    TimeProvider timeProvider) : IWorkItemService
{
    public async Task<IReadOnlyList<WorkItemDto>> GetAllAsync(
        WorkItemStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var workItems = await workItemRepository.GetAllWithDetailsAsync(status, cancellationToken);
        return workItems.Select(w => w.ToDto()).ToList();
    }

    public async Task<WorkItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var workItem = await GetWorkItemAsync(id, cancellationToken);
        return workItem.ToDto();
    }

    public async Task<WorkItemDto> CreateAsync(
        WorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await requestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var workItem = new WorkItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeDescription(request.Description)
        };

        await workItemRepository.AddAsync(workItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workItem.ToDto();
    }

    public async Task<WorkItemDto> UpdateAsync(
        int id,
        WorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await requestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var workItem = await GetWorkItemAsync(id, cancellationToken);

        if (workItem.Status == WorkItemStatus.Completed)
        {
            throw new ConflictException("Completed work items cannot be edited.");
        }

        workItem.Title = request.Title.Trim();
        workItem.Description = NormalizeDescription(request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workItem.ToDto();
    }

    public async Task<WorkItemDto> AnalyzeAsync(
        int id,
        AnalyzeWorkItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await analyzeValidator.ValidateAndThrowAsync(request, cancellationToken);

        var workItem = await GetWorkItemAsync(id, cancellationToken);

        var taskType = await taskTypeRepository.GetByIdAsync(request.TaskTypeId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskType), request.TaskTypeId);

        workItem.Analyze(taskType, timeProvider.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workItem.ToDto();
    }

    public async Task<WorkItemDto> CompleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var workItem = await GetWorkItemAsync(id, cancellationToken);

        workItem.Complete(timeProvider.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workItem.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var workItem = await GetWorkItemAsync(id, cancellationToken);

        if (workItem.Status is WorkItemStatus.Assigned or WorkItemStatus.Completed)
        {
            throw new ConflictException(
                "Assigned or completed work items are part of the distribution history and cannot be deleted.");
        }

        workItemRepository.Remove(workItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<WorkItem> GetWorkItemAsync(int id, CancellationToken cancellationToken) =>
        await workItemRepository.GetByIdWithDetailsAsync(id, cancellationToken)
        ?? throw new NotFoundException(nameof(WorkItem), id);

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}