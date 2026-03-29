using FlatFlow.Domain.Enums;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.UpdateChore;

public record UpdateChoreCommand(
    Guid ChoreId,
    string Title,
    string Description,
    ChoreFrequency Frequency) : IRequest<Unit>;
