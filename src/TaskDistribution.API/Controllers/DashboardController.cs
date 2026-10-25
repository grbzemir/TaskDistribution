using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Application.Features.Dashboard;

namespace TaskDistribution.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DashboardSummaryDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> Get(CancellationToken cancellationToken) =>
        Ok(await dashboardService.GetSummaryAsync(cancellationToken));
}