using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public record JoinFlatCommand(
    string AccessCode,
    string FirstName,
    string LastName,
    string Email,
    string UserId) : IRequest<Guid>;
