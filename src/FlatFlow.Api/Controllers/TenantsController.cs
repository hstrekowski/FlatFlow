using FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;
using FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;
using FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantById;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantsByFlatId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatFlow.Api.Controllers;

[ApiController]
[Route("api/flats/{flatId:guid}/tenants")]
[Authorize(Policy = "FlatMember")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/flats/{flatId}/tenants
    [HttpGet("")]
    [ProducesResponseType(typeof(List<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TenantDto>>> GetByFlatId([FromRoute] Guid flatId)
    {
        var result = await _mediator.Send(new GetTenantsByFlatIdQuery(flatId));
        return Ok(result);
    }

    // GET api/flats/{flatId}/tenants/{tenantId}
    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetById([FromRoute] Guid flatId, [FromRoute] Guid tenantId)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(tenantId));
        return Ok(result);
    }

    // POST api/flats/{flatId}/tenants/{tenantId}/promote
    [Authorize(Policy = "FlatOwner")]
    [HttpPost("{tenantId:guid}/promote")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Promote([FromRoute] Guid flatId, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new PromoteTenantCommand(tenantId));
        return NoContent();
    }

    // POST api/flats/{flatId}/tenants/{tenantId}/revoke-ownership
    [Authorize(Policy = "FlatOwner")]
    [HttpPost("{tenantId:guid}/revoke-ownership")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> RevokeOwnership([FromRoute] Guid flatId, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RevokeTenantOwnershipCommand(tenantId));
        return NoContent();
    }

    // DELETE api/flats/{flatId}/tenants/{tenantId}
    [Authorize(Policy = "FlatOwner")]
    [HttpDelete("{tenantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove([FromRoute] Guid flatId, [FromRoute] Guid tenantId)
    {
        await _mediator.Send(new RemoveTenantCommand(flatId, tenantId));
        return NoContent();
    }
}
