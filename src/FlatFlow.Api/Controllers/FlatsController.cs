using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Flat.Commands.CreateFlat;
using FlatFlow.Application.Features.Flat.Commands.DeleteFlat;
using FlatFlow.Application.Features.Flat.Commands.RefreshAccessCode;
using FlatFlow.Application.Features.Flat.Commands.UpdateFlat;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using FlatFlow.Application.Features.Flat.Queries.GetAllFlats;
using FlatFlow.Application.Features.Flat.Queries.GetFlatByAccessCode;
using FlatFlow.Application.Features.Flat.Queries.GetFlatById;
using FlatFlow.Application.Features.Flat.Queries.GetFlatsByUserId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats")]
public class FlatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateFlatCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<FlatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<FlatDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllFlatsQuery(page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FlatDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlatDetailDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetFlatByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("by-access-code/{code}")]
    [ProducesResponseType(typeof(FlatDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlatDto>> GetByAccessCode(string code)
    {
        var result = await _mediator.Send(new GetFlatByAccessCodeQuery(code));
        return Ok(result);
    }

    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(typeof(List<FlatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FlatDto>>> GetByUserId(string userId)
    {
        var result = await _mediator.Send(new GetFlatsByUserIdQuery(userId));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, UpdateFlatCommand command)
    {
        if (id != command.FlatId)
            return BadRequest(new { message = "Route id does not match command id." });

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteFlatCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/refresh-access-code")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RefreshAccessCode(Guid id)
    {
        await _mediator.Send(new RefreshAccessCodeCommand(id));
        return NoContent();
    }
}
