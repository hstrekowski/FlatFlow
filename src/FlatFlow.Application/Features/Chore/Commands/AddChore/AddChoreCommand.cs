using FlatFlow.Domain.Enums;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Commands.AddChore;

public record AddChoreCommand(
    Guid FlatId,
    string Title,
    string Description,
    ChoreFrequency Frequency,
    Guid CreatedById) : IRequest<Guid>;
