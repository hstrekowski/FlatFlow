using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;

public record PromoteTenantCommand(Guid TenantId) : IRequest<Unit>;
