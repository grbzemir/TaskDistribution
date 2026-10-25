using Microsoft.AspNetCore.Mvc;

using TaskDistribution.Application.Features.Assignments;

namespace TaskDistribution.API.Controllers;

[ApiController]
[Route("api/assignments")]
[Produces("application/json")]
public sealed class AssignmentsController(IAssignmentQueryService assignmentQueryService) : ControllerBase
{
    [HttpGet("rules")]
    [ProducesResponseType<IReadOnlyList<AssignmentRuleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AssignmentRuleDto>>> GetRules(CancellationToken cancellationToken) =>
        Ok(await assignmentQueryService.GetRulesAsync(cancellationToken));

    [HttpGet("logs")]
    [ProducesResponseType<IReadOnlyList<AssignmentLogDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AssignmentLogDto>>> GetLogs(
        [FromQuery] int count = 50,
        CancellationToken cancellationToken = default) =>
        Ok(await assignmentQueryService.GetRecentLogsAsync(count, cancellationToken));
}