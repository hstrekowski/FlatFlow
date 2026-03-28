using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;

public class PromoteTenantCommandHandler : IRequestHandler<PromoteTenantCommand, Unit>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<PromoteTenantCommandHandler> _logger;

    public PromoteTenantCommandHandler(ITenantRepository tenantRepository, ILogger<PromoteTenantCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(PromoteTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Tenant), request.TenantId);

        tenant.PromoteToOwner();

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} promoted to owner", tenant.Id);

        return Unit.Value;
    }
}
