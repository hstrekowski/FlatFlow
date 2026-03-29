using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;

public class RemoveChoreAssignmentCommandHandler : IRequestHandler<RemoveChoreAssignmentCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ILogger<RemoveChoreAssignmentCommandHandler> _logger;

    public RemoveChoreAssignmentCommandHandler(IChoreRepository choreRepository, ILogger<RemoveChoreAssignmentCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveChoreAssignmentCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        chore.RemoveAssignment(request.AssignmentId);

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Assignment {AssignmentId} removed from Chore {ChoreId}", request.AssignmentId, chore.Id);

        return Unit.Value;
    }
}
