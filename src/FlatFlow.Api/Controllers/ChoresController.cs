using FlatFlow.Application.Features.Chore.Commands.AddChore;
using FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;
using FlatFlow.Application.Features.Chore.Commands.CompleteChoreAssignment;
using FlatFlow.Application.Features.Chore.Commands.RemoveChore;
using FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;
using FlatFlow.Application.Features.Chore.Commands.ReopenChoreAssignment;
using FlatFlow.Application.Features.Chore.Commands.UpdateChore;
using FlatFlow.Application.Features.Chore.Queries.DTOs;
using FlatFlow.Application.Features.Chore.Queries.GetChoreById;
using FlatFlow.Application.Features.Chore.Queries.GetChoresByFlatId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/chores")]
public class ChoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Add(Guid flatId, AddChoreCommand command)
    {
        if (flatId != command.FlatId)
            return BadRequest(new { message = "Route flatId does not match command flatId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, choreId = id }, id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ChoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ChoreDto>>> GetByFlatId(Guid flatId)
    {
        var result = await _mediator.Send(new GetChoresByFlatIdQuery(flatId));
        return Ok(result);
    }

    [HttpGet("{choreId:guid}")]
    [ProducesResponseType(typeof(ChoreDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChoreDetailDto>> GetById(Guid flatId, Guid choreId)
    {
        var result = await _mediator.Send(new GetChoreByIdQuery(choreId));
        return Ok(result);
    }

    [HttpPut("{choreId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid flatId, Guid choreId, UpdateChoreCommand command)
    {
        if (choreId != command.ChoreId)
            return BadRequest(new { message = "Route choreId does not match command choreId." });

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{choreId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove(Guid flatId, Guid choreId)
    {
        await _mediator.Send(new RemoveChoreCommand(flatId, choreId));
        return NoContent();
    }

    [HttpPost("{choreId:guid}/assignments")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddAssignment(Guid flatId, Guid choreId, AddChoreAssignmentCommand command)
    {
        if (choreId != command.ChoreId)
            return BadRequest(new { message = "Route choreId does not match command choreId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, choreId }, id);
    }

    [HttpDelete("{choreId:guid}/assignments/{assignmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveAssignment(Guid flatId, Guid choreId, Guid assignmentId)
    {
        await _mediator.Send(new RemoveChoreAssignmentCommand(choreId, assignmentId));
        return NoContent();
    }

    [HttpPost("{choreId:guid}/assignments/{assignmentId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CompleteAssignment(Guid flatId, Guid choreId, Guid assignmentId)
    {
        await _mediator.Send(new CompleteChoreAssignmentCommand(choreId, assignmentId));
        return NoContent();
    }

    [HttpPost("{choreId:guid}/assignments/{assignmentId:guid}/reopen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ReopenAssignment(Guid flatId, Guid choreId, Guid assignmentId)
    {
        await _mediator.Send(new ReopenChoreAssignmentCommand(choreId, assignmentId));
        return NoContent();
    }
}
