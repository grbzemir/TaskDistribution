using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskDistribution.Application.Features.Employees;
using TaskDistribution.Application.Features.Roles;
using TaskDistribution.Application.Features.TaskTypes;

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
        return services;
    }
}