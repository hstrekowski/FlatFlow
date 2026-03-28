using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;

public class RevokeTenantOwnershipCommandHandler : IRequestHandler<RevokeTenantOwnershipCommand, Unit>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<RevokeTenantOwnershipCommandHandler> _logger;

    public RevokeTenantOwnershipCommandHandler(ITenantRepository tenantRepository, ILogger<RevokeTenantOwnershipCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RevokeTenantOwnershipCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Tenant), request.TenantId);

        tenant.RevokeOwnership();

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} ownership revoked", tenant.Id);

        return Unit.Value;
    }
}
