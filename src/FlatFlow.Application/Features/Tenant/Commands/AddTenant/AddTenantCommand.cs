using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.AddTenant;

public record AddTenantCommand(
    Guid FlatId,
    string FirstName,
    string LastName,
    string Email,
    string UserId,
    bool IsOwner = false) : IRequest<Guid>;
