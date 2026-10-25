using System;
using System.Collections.Generic;
using System.Text;

using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Abstractions.Persistence;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IReadOnlyList<Employee>> GetAllWithRoleAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetActiveByRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default);
}