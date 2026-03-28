using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;

public record RevokeTenantOwnershipCommand(Guid TenantId) : IRequest<Unit>;
