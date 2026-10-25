using Microsoft.EntityFrameworkCore;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository(TaskDistributionDbContext dbContext)
    : Repository<Employee>(dbContext), IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllWithRoleAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(e => e.Role)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(cancellationToken);

    public Task<Employee?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetActiveByRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(e => e.Role)
            .Where(e => e.IsActive && e.Role.Name == roleName)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
}