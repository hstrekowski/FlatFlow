using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.CompleteChoreAssignment;

public class CompleteChoreAssignmentCommandHandler : IRequestHandler<CompleteChoreAssignmentCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ILogger<CompleteChoreAssignmentCommandHandler> _logger;

    public CompleteChoreAssignmentCommandHandler(IChoreRepository choreRepository, ILogger<CompleteChoreAssignmentCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(CompleteChoreAssignmentCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var assignment = chore.ChoreAssignments.FirstOrDefault(a => a.Id == request.AssignmentId)
            ?? throw new NotFoundException(nameof(Domain.Entities.ChoreAssignment), request.AssignmentId);

        assignment.Complete();

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Assignment {AssignmentId} completed in Chore {ChoreId}", request.AssignmentId, chore.Id);

        return Unit.Value;
    }
}
