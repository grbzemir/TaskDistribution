using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Common;

namespace TaskDistribution.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(TaskDistributionDbContext dbContext) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected TaskDistributionDbContext DbContext { get; } = dbContext;

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await DbSet.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(predicate, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Remove(TEntity entity) => DbSet.Remove(entity);
}