using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;

public record RemoveChoreAssignmentCommand(Guid ChoreId, Guid AssignmentId) : IRequest<Unit>;
