using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.ReopenChoreAssignment;

public class ReopenChoreAssignmentCommandHandler : IRequestHandler<ReopenChoreAssignmentCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ReopenChoreAssignmentCommandHandler> _logger;

    public ReopenChoreAssignmentCommandHandler(
        IChoreRepository choreRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<ReopenChoreAssignmentCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ReopenChoreAssignmentCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var assignment = chore.ChoreAssignments.FirstOrDefault(a => a.Id == request.AssignmentId)
            ?? throw new NotFoundException(nameof(Domain.Entities.ChoreAssignment), request.AssignmentId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, chore.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && assignment.TenantId != currentTenant.Id)
            throw new ForbiddenException("You can only reopen your own assignments.");

        assignment.Reopen();

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Assignment {AssignmentId} reopened in Chore {ChoreId}", request.AssignmentId, chore.Id);

        return Unit.Value;
    }
}
