using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.ReopenChoreAssignment;

public record ReopenChoreAssignmentCommand(Guid ChoreId, Guid AssignmentId) : IRequest<Unit>;
