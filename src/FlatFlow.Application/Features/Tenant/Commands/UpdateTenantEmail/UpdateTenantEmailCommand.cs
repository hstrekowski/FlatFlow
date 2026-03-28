using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;

public record UpdateTenantEmailCommand(Guid TenantId, string Email) : IRequest<Unit>;
