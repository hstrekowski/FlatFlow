using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.UpdateTenantProfile;

public record UpdateTenantProfileCommand(Guid TenantId, string FirstName, string LastName) : IRequest<Unit>;
