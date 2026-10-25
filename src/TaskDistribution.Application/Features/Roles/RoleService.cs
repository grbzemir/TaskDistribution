using TaskDistribution.Application.Abstractions.Persistence;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Application.Features.Roles;

public sealed class RoleService(IRepository<Role> roleRepository) : IRoleService
{
    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);

        return roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToList();
    }
}