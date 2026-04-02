using MediatR;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public record JoinFlatCommand(string AccessCode) : IRequest<Guid>;
