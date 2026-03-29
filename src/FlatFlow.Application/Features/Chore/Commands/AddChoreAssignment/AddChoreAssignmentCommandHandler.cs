using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;

public class AddChoreAssignmentCommandHandler : IRequestHandler<AddChoreAssignmentCommand, Guid>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ILogger<AddChoreAssignmentCommandHandler> _logger;

    public AddChoreAssignmentCommandHandler(IChoreRepository choreRepository, ILogger<AddChoreAssignmentCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddChoreAssignmentCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var assignment = chore.AddAssignment(request.TenantId, request.DueDate);

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Assignment added to Chore {ChoreId} for Tenant {TenantId}", chore.Id, request.TenantId);

        return assignment.Id;
    }
}
