using MediatR;

namespace FlatFlow.Application.Features.Flat.Commands.RefreshAccessCode;

public record RefreshAccessCodeCommand(Guid FlatId) : IRequest<Unit>;
