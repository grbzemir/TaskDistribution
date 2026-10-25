using FluentValidation;

namespace TaskDistribution.Application.Features.WorkItems;

public sealed class WorkItemRequestValidator : AbstractValidator<WorkItemRequest>
{
    public WorkItemRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public sealed class AnalyzeWorkItemRequestValidator : AbstractValidator<AnalyzeWorkItemRequest>
{
    public AnalyzeWorkItemRequestValidator()
    {
        RuleFor(x => x.TaskTypeId).GreaterThan(0);
    }
}