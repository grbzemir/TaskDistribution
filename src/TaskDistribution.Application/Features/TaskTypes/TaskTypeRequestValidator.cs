using FluentValidation;
using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Application.Features.TaskTypes;

public sealed class TaskTypeRequestValidator : AbstractValidator<TaskTypeRequest>
{
    public TaskTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Difficulty).InclusiveBetween(DifficultyLevel.Min, DifficultyLevel.Max);
    }
}