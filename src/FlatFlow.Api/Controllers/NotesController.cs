using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Note.Commands.AddNote;
using FlatFlow.Application.Features.Note.Commands.RemoveNote;
using FlatFlow.Application.Features.Note.Commands.UpdateNote;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using FlatFlow.Application.Features.Note.Queries.GetNoteById;
using FlatFlow.Application.Features.Note.Queries.GetNotesByFlatId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/notes")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/flats/{flatId}/notes
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Add([FromRoute] Guid flatId, [FromBody] AddNoteCommand command)
    {
        if (flatId != command.FlatId)
            return BadRequest(new { message = "Route flatId does not match command flatId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, noteId = id }, id);
    }

    // GET api/flats/{flatId}/notes?page=1&pageSize=10
    [HttpGet("")]
    [ProducesResponseType(typeof(PaginatedResult<NoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<NoteDto>>> GetByFlatId([FromRoute] Guid flatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetNotesByFlatIdQuery(flatId, page, pageSize));
        return Ok(result);
    }

    // GET api/flats/{flatId}/notes/{noteId}
    [HttpGet("{noteId:guid}")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoteDto>> GetById([FromRoute] Guid flatId, [FromRoute] Guid noteId)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(noteId));
        return Ok(result);
    }

    // PUT api/flats/{flatId}/notes/{noteId}
    [HttpPut("{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update([FromRoute] Guid flatId, [FromRoute] Guid noteId, [FromBody] UpdateNoteCommand command)
    {
        if (noteId != command.NoteId)
            return BadRequest(new { message = "Route noteId does not match command noteId." });

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/flats/{flatId}/notes/{noteId}
    [HttpDelete("{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove([FromRoute] Guid flatId, [FromRoute] Guid noteId)
    {
        await _mediator.Send(new RemoveNoteCommand(flatId, noteId));
        return NoContent();
    }
}
