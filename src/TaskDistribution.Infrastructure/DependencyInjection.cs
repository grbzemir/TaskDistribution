using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Infrastructure.Persistence;
using TaskDistribution.Infrastructure.Persistence.Interceptors;
using TaskDistribution.Infrastructure.Persistence.Repositories;

namespace TaskDistribution.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringName = "DefaultConnection";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured.");

        var resolvedConnectionString = ResolveSqlitePath(connectionString, contentRootPath);

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<TaskDistributionDbContext>((serviceProvider, options) =>
            options
                .UseSqlite(resolvedConnectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>()));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<IAssignmentLogRepository, AssignmentLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static string ResolveSqlitePath(string connectionString, string basePath)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (builder.DataSource != ":memory:" && !Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.Combine(basePath, builder.DataSource);
        }

        return builder.ToString();
    }
}