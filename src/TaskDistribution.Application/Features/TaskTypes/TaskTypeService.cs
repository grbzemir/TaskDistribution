using FluentValidation;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Application.Common.Exceptions;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.TaskTypes;

public sealed class TaskTypeService(
    IRepository<TaskType> taskTypeRepository,
    IWorkItemRepository workItemRepository,
    IUnitOfWork unitOfWork,
    IValidator<TaskTypeRequest> validator) : ITaskTypeService
{
    public async Task<IReadOnlyList<TaskTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var taskTypes = await taskTypeRepository.GetAllAsync(cancellationToken);

        return taskTypes
            .OrderBy(t => t.Difficulty)
            .Select(t => t.ToDto())
            .ToList();
    }

    public async Task<TaskTypeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var taskType = await taskTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskType), id);

        return taskType.ToDto();
    }

    public async Task<TaskTypeDto> CreateAsync(
        TaskTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureUniqueAsync(request, excludedId: 0, cancellationToken);

        var taskType = new TaskType
        {
            Name = request.Name.Trim(),
            Description = NormalizeDescription(request.Description),
            Difficulty = request.Difficulty
        };

        await taskTypeRepository.AddAsync(taskType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return taskType.ToDto();
    }

    public async Task<TaskTypeDto> UpdateAsync(
        int id,
        TaskTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var taskType = await taskTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskType), id);

        await EnsureUniqueAsync(request, excludedId: id, cancellationToken);

        taskType.Name = request.Name.Trim();
        taskType.Description = NormalizeDescription(request.Description);
        taskType.Difficulty = request.Difficulty;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return taskType.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var taskType = await taskTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskType), id);

        var isInUse = await workItemRepository.AnyAsync(w => w.TaskTypeId == id, cancellationToken);

        if (isInUse)
        {
            throw new ConflictException("This task type is used by work items and cannot be deleted.");
        }

        taskTypeRepository.Remove(taskType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueAsync(
        TaskTypeRequest request,
        int excludedId,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (await taskTypeRepository.AnyAsync(t => t.Name == name && t.Id != excludedId, cancellationToken))
        {
            throw new ConflictException($"A task type named '{name}' already exists.");
        }

        if (await taskTypeRepository.AnyAsync(
                t => t.Difficulty == request.Difficulty && t.Id != excludedId,
                cancellationToken))
        {
            throw new ConflictException(
                $"Another task type already uses difficulty level {request.Difficulty}.");
        }
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}