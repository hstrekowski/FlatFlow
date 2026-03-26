using MediatR;

namespace FlatFlow.Application.Features.Flat.Commands.DeleteFlat;

public record DeleteFlatCommand(Guid FlatId) : IRequest<Unit>;
