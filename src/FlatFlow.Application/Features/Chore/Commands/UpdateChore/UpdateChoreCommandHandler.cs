using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.UpdateChore;

public class UpdateChoreCommandHandler : IRequestHandler<UpdateChoreCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateChoreCommandHandler> _logger;

    public UpdateChoreCommandHandler(
        IChoreRepository choreRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateChoreCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateChoreCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, chore.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && chore.CreatedById != currentTenant.Id)
            throw new ForbiddenException("You can only modify your own chores.");

        chore.UpdateTitle(request.Title);
        chore.UpdateDescription(request.Description);
        chore.UpdateFrequency(request.Frequency);

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Chore {ChoreId} updated", chore.Id);

        return Unit.Value;
    }
}
