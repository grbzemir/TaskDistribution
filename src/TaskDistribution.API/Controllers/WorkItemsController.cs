using Microsoft.AspNetCore.Mvc;

using TaskDistribution.Application.Features.Assignments;
using TaskDistribution.Application.Features.WorkItems;
using TaskDistribution.Domain.Enums;

namespace TaskDistribution.API.Controllers;

[ApiController]
[Route("api/work-items")]
[Produces("application/json")]
public sealed class WorkItemsController(
    IWorkItemService workItemService,
    IWorkItemAssignmentService assignmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<WorkItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkItemDto>>> GetAll(
        [FromQuery] WorkItemStatus? status,
        CancellationToken cancellationToken) =>
        Ok(await workItemService.GetAllAsync(status, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await workItemService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkItemDto>> Create(
        WorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var workItem = await workItemService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = workItem.Id }, workItem);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WorkItemDto>> Update(
        int id,
        WorkItemRequest request,
        CancellationToken cancellationToken) =>
        Ok(await workItemService.UpdateAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/analyze")]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> Analyze(
        int id,
        AnalyzeWorkItemRequest request,
        CancellationToken cancellationToken) =>
        Ok(await workItemService.AnalyzeAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/assign")]
    [ProducesResponseType<AssignmentResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AssignmentResultDto>> Assign(int id, CancellationToken cancellationToken) =>
        Ok(await assignmentService.AssignAsync(id, cancellationToken));

    [HttpPost("{id:int}/complete")]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> Complete(int id, CancellationToken cancellationToken) =>
        Ok(await workItemService.CompleteAsync(id, cancellationToken));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await workItemService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}