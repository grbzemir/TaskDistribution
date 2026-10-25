using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskDistribution.Application.Features.Assignments.Rules;
using TaskDistribution.Application.Features.Assignments.Selection;
using TaskDistribution.Application.Features.Assignments.Settings;
using TaskDistribution.Application.Features.Employees;
using TaskDistribution.Application.Features.Roles;
using TaskDistribution.Application.Features.TaskTypes;
using TaskDistribution.Application.Features.WorkItems;

namespace TaskDistribution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ITaskTypeService, TaskTypeService>();
        services.AddScoped<IWorkItemService, WorkItemService>();
        services.AddSingleton<IAssignmentRule, ConsecutiveDifficultyRule>();
        services.AddSingleton<IAssignmentRule, ConsecutiveAssignmentRule>();
        services.AddSingleton<IAssignmentRule, ConsecutiveHardTaskRule>();
        services.AddSingleton<IAssignmentSettingsProvider, OptionsAssignmentSettingsProvider>();
        services.AddSingleton<IDeveloperSelector, FairDeveloperSelector>();
        return services;
    }
}