using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChore;

public class RemoveChoreCommandHandler : IRequestHandler<RemoveChoreCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RemoveChoreCommandHandler> _logger;

    public RemoveChoreCommandHandler(
        IFlatRepository flatRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<RemoveChoreCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveChoreCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithChoresAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var chore = flat.Chores.FirstOrDefault(c => c.Id == request.ChoreId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, request.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && chore.CreatedById != currentTenant.Id)
            throw new ForbiddenException("You can only remove your own chores.");

        flat.RemoveChore(request.ChoreId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Chore {ChoreId} removed from Flat {FlatId}", request.ChoreId, flat.Id);

        return Unit.Value;
    }
}
