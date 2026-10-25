using Microsoft.AspNetCore.Mvc;

using TaskDistribution.Application.Features.TaskTypes;

namespace TaskDistribution.API.Controllers;

[ApiController]
[Route("api/task-types")]
[Produces("application/json")]
public sealed class TaskTypesController(ITaskTypeService taskTypeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskTypeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskTypeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await taskTypeService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType<TaskTypeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskTypeDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await taskTypeService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType<TaskTypeDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TaskTypeDto>> Create(
        TaskTypeRequest request,
        CancellationToken cancellationToken)
    {
        var taskType = await taskTypeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = taskType.Id }, taskType);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<TaskTypeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TaskTypeDto>> Update(
        int id,
        TaskTypeRequest request,
        CancellationToken cancellationToken) =>
        Ok(await taskTypeService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await taskTypeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}