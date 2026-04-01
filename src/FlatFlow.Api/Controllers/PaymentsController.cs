using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Payment.Commands.AddPayment;
using FlatFlow.Application.Features.Payment.Commands.AddPaymentShare;
using FlatFlow.Application.Features.Payment.Commands.MarkShareAsPaid;
using FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;
using FlatFlow.Application.Features.Payment.Commands.RemovePayment;
using FlatFlow.Application.Features.Payment.Commands.RemovePaymentShare;
using FlatFlow.Application.Features.Payment.Commands.UpdatePayment;
using FlatFlow.Application.Features.Payment.Queries.DTOs;
using FlatFlow.Application.Features.Payment.Queries.GetPaymentById;
using FlatFlow.Application.Features.Payment.Queries.GetPaymentsByFlatId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/payments")]
[Authorize(Policy = "FlatMember")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/flats/{flatId}/payments
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Add([FromRoute] Guid flatId, [FromBody] AddPaymentCommand command)
    {
        if (flatId != command.FlatId)
            return BadRequest(new { message = "Route flatId does not match command flatId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, paymentId = id }, id);
    }

    // GET api/flats/{flatId}/payments?page=1&pageSize=10
    [HttpGet("")]
    [ProducesResponseType(typeof(PaginatedResult<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<PaymentDto>>> GetByFlatId([FromRoute] Guid flatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPaymentsByFlatIdQuery(flatId, page, pageSize));
        return Ok(result);
    }

    // GET api/flats/{flatId}/payments/{paymentId}
    [HttpGet("{paymentId:guid}")]
    [ProducesResponseType(typeof(PaymentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDetailDto>> GetById([FromRoute] Guid flatId, [FromRoute] Guid paymentId)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(paymentId));
        return Ok(result);
    }

    // PUT api/flats/{flatId}/payments/{paymentId}
    [HttpPut("{paymentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update([FromRoute] Guid flatId, [FromRoute] Guid paymentId, [FromBody] UpdatePaymentCommand command)
    {
        if (paymentId != command.PaymentId)
            return BadRequest(new { message = "Route paymentId does not match command paymentId." });

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/flats/{flatId}/payments/{paymentId}
    [HttpDelete("{paymentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove([FromRoute] Guid flatId, [FromRoute] Guid paymentId)
    {
        await _mediator.Send(new RemovePaymentCommand(flatId, paymentId));
        return NoContent();
    }

    // POST api/flats/{flatId}/payments/{paymentId}/shares
    [HttpPost("{paymentId:guid}/shares")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddShare([FromRoute] Guid flatId, [FromRoute] Guid paymentId, [FromBody] AddPaymentShareCommand command)
    {
        if (paymentId != command.PaymentId)
            return BadRequest(new { message = "Route paymentId does not match command paymentId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, paymentId }, id);
    }

    // DELETE api/flats/{flatId}/payments/{paymentId}/shares/{shareId}
    [HttpDelete("{paymentId:guid}/shares/{shareId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveShare([FromRoute] Guid flatId, [FromRoute] Guid paymentId, [FromRoute] Guid shareId)
    {
        await _mediator.Send(new RemovePaymentShareCommand(paymentId, shareId));
        return NoContent();
    }

    // POST api/flats/{flatId}/payments/{paymentId}/shares/{shareId}/mark-paid
    [HttpPost("{paymentId:guid}/shares/{shareId:guid}/mark-paid")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> MarkShareAsPaid([FromRoute] Guid flatId, [FromRoute] Guid paymentId, [FromRoute] Guid shareId)
    {
        await _mediator.Send(new MarkShareAsPaidCommand(paymentId, shareId));
        return NoContent();
    }

    // POST api/flats/{flatId}/payments/{paymentId}/shares/{shareId}/mark-partial
    [HttpPost("{paymentId:guid}/shares/{shareId:guid}/mark-partial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> MarkShareAsPartial([FromRoute] Guid flatId, [FromRoute] Guid paymentId, [FromRoute] Guid shareId)
    {
        await _mediator.Send(new MarkShareAsPartialCommand(paymentId, shareId));
        return NoContent();
    }
}
