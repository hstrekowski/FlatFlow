using FlatFlow.Application.Features.Tenant.Commands.AddTenant;
using FlatFlow.Application.Features.Tenant.Commands.JoinFlat;
using FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;
using FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;
using FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;
using FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;
using FlatFlow.Application.Features.Tenant.Commands.UpdateTenantProfile;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantById;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantsByFlatId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<Guid>> Add(Guid flatId, AddTenantCommand command)
    {
        if (flatId != command.FlatId)
            return BadRequest(new { message = "Route flatId does not match command flatId." });

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, tenantId = id }, id);
    }

    [HttpPost("join")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Join(Guid flatId, JoinFlatCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { flatId, tenantId = id }, id);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TenantDto>>> GetByFlatId(Guid flatId)
    {
        var result = await _mediator.Send(new GetTenantsByFlatIdQuery(flatId));
        return Ok(result);
    }

    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetById(Guid flatId, Guid tenantId)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(tenantId));
        return Ok(result);
    }

    [HttpPut("{tenantId:guid}/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProfile(Guid flatId, Guid tenantId, UpdateTenantProfileCommand command)
    {
        if (tenantId != command.TenantId)
            return BadRequest(new { message = "Route tenantId does not match command tenantId." });

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{tenantId:guid}/email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateEmail(Guid flatId, Guid tenantId, UpdateTenantEmailCommand command)
    {
        if (tenantId != command.TenantId)
            return BadRequest(new { message = "Route tenantId does not match command tenantId." });

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{tenantId:guid}/promote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Promote(Guid flatId, Guid tenantId)
    {
        await _mediator.Send(new PromoteTenantCommand(tenantId));
        return NoContent();
    }

    [HttpPost("{tenantId:guid}/revoke-ownership")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> RevokeOwnership(Guid flatId, Guid tenantId)
    {
        await _mediator.Send(new RevokeTenantOwnershipCommand(tenantId));
        return NoContent();
    }

    [HttpDelete("{tenantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove(Guid flatId, Guid tenantId)
    {
        await _mediator.Send(new RemoveTenantCommand(flatId, tenantId));
        return NoContent();
    }
}
