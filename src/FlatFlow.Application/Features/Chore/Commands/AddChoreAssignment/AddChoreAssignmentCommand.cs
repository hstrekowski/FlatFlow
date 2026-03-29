using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;

public record AddChoreAssignmentCommand(
    Guid ChoreId,
    Guid TenantId,
    DateTime DueDate) : IRequest<Guid>;
