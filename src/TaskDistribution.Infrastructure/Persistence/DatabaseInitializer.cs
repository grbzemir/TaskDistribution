using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TaskDistribution.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task MigrateDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskDistributionDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}