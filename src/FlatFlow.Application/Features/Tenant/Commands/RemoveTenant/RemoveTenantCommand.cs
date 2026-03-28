using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;

public record RemoveTenantCommand(Guid FlatId, Guid TenantId) : IRequest<Unit>;
