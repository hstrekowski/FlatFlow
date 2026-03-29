using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChore;

public record RemoveChoreCommand(Guid FlatId, Guid ChoreId) : IRequest<Unit>;
