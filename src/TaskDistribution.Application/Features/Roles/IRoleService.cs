using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDistribution.Application.Features.Roles;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
}