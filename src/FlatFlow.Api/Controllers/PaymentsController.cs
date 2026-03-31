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
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Add(Guid flatId, AddPaymentCommand command)
    {
        if (flatId != command.FlatId)
            return BadRequest(new { message = "Route flatId does not match command flatId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, paymentId = id }, id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<PaymentDto>>> GetByFlatId(Guid flatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPaymentsByFlatIdQuery(flatId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{paymentId:guid}")]
    [ProducesResponseType(typeof(PaymentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDetailDto>> GetById(Guid flatId, Guid paymentId)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(paymentId));
        return Ok(result);
    }

    [HttpPut("{paymentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid flatId, Guid paymentId, UpdatePaymentCommand command)
    {
        if (paymentId != command.PaymentId)
            return BadRequest(new { message = "Route paymentId does not match command paymentId." });

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{paymentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove(Guid flatId, Guid paymentId)
    {
        await _mediator.Send(new RemovePaymentCommand(flatId, paymentId));
        return NoContent();
    }

    [HttpPost("{paymentId:guid}/shares")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddShare(Guid flatId, Guid paymentId, AddPaymentShareCommand command)
    {
        if (paymentId != command.PaymentId)
            return BadRequest(new { message = "Route paymentId does not match command paymentId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, paymentId }, id);
    }

    [HttpDelete("{paymentId:guid}/shares/{shareId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveShare(Guid flatId, Guid paymentId, Guid shareId)
    {
        await _mediator.Send(new RemovePaymentShareCommand(paymentId, shareId));
        return NoContent();
    }

    [HttpPost("{paymentId:guid}/shares/{shareId:guid}/mark-paid")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> MarkShareAsPaid(Guid flatId, Guid paymentId, Guid shareId)
    {
        await _mediator.Send(new MarkShareAsPaidCommand(paymentId, shareId));
        return NoContent();
    }

    [HttpPost("{paymentId:guid}/shares/{shareId:guid}/mark-partial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> MarkShareAsPartial(Guid flatId, Guid paymentId, Guid shareId)
    {
        await _mediator.Send(new MarkShareAsPartialCommand(paymentId, shareId));
        return NoContent();
    }
}
