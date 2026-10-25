using Microsoft.AspNetCore.Mvc;

using TaskDistribution.Application.Features.Roles;

namespace TaskDistribution.API.Controllers;

[ApiController]
[Route("api/roles")]
[Produces("application/json")]
public sealed class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RoleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await roleService.GetAllAsync(cancellationToken));
}