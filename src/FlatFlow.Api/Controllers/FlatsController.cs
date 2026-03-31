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
using FlatFlow.Application.Features.Tenant.Commands.JoinFlat;
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

    // POST api/flats
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateFlatCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // GET api/flats?page=1&pageSize=10
    [HttpGet("")]
    [ProducesResponseType(typeof(PaginatedResult<FlatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<FlatDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllFlatsQuery(page, pageSize));
        return Ok(result);
    }

    // GET api/flats/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FlatDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlatDetailDto>> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetFlatByIdQuery(id));
        return Ok(result);
    }

    // GET api/flats/by-access-code/{code}
    [HttpGet("by-access-code/{code}")]
    [ProducesResponseType(typeof(FlatDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlatDto>> GetByAccessCode([FromRoute] string code)
    {
        var result = await _mediator.Send(new GetFlatByAccessCodeQuery(code));
        return Ok(result);
    }

    // GET api/flats/by-user/{userId}
    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(typeof(List<FlatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FlatDto>>> GetByUserId([FromRoute] string userId)
    {
        var result = await _mediator.Send(new GetFlatsByUserIdQuery(userId));
        return Ok(result);
    }

    // PUT api/flats/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateFlatCommand command)
    {
        if (id != command.FlatId)
            return BadRequest(new { message = "Route id does not match command id." });

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/flats/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteFlatCommand(id));
        return NoContent();
    }

    // POST api/flats/{id}/refresh-access-code
    [HttpPost("{id:guid}/refresh-access-code")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RefreshAccessCode([FromRoute] Guid id)
    {
        await _mediator.Send(new RefreshAccessCodeCommand(id));
        return NoContent();
    }

    // POST api/flats/join
    [HttpPost("join")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Join([FromBody] JoinFlatCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
