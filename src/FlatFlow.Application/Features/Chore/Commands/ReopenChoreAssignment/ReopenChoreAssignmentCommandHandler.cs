using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.ReopenChoreAssignment;

public class ReopenChoreAssignmentCommandHandler : IRequestHandler<ReopenChoreAssignmentCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ILogger<ReopenChoreAssignmentCommandHandler> _logger;

    public ReopenChoreAssignmentCommandHandler(IChoreRepository choreRepository, ILogger<ReopenChoreAssignmentCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(ReopenChoreAssignmentCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var assignment = chore.ChoreAssignments.FirstOrDefault(a => a.Id == request.AssignmentId)
            ?? throw new NotFoundException(nameof(Domain.Entities.ChoreAssignment), request.AssignmentId);

        assignment.Reopen();

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Assignment {AssignmentId} reopened in Chore {ChoreId}", request.AssignmentId, chore.Id);

        return Unit.Value;
    }
}
