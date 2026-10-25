using FluentValidation;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.Employees;

public sealed class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(IRepository<Role> roleRepository)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

        RuleFor(x => x.RoleId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .MustAsync((roleId, ct) => roleRepository.AnyAsync(r => r.Id == roleId, ct))
            .WithMessage("Selected role does not exist.");
    }
}

public sealed class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(IRepository<Role> roleRepository)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

        RuleFor(x => x.RoleId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .MustAsync((roleId, ct) => roleRepository.AnyAsync(r => r.Id == roleId, ct))
            .WithMessage("Selected role does not exist.");
    }
}