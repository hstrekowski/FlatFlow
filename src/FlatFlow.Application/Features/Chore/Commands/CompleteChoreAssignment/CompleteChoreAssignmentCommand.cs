using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.CompleteChoreAssignment;

public record CompleteChoreAssignmentCommand(Guid ChoreId, Guid AssignmentId) : IRequest<Unit>;
