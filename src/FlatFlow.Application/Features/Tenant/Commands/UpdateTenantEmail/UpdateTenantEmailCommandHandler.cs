using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;

public class UpdateTenantEmailCommandHandler : IRequestHandler<UpdateTenantEmailCommand, Unit>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<UpdateTenantEmailCommandHandler> _logger;

    public UpdateTenantEmailCommandHandler(ITenantRepository tenantRepository, ILogger<UpdateTenantEmailCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateTenantEmailCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Tenant), request.TenantId);

        tenant.UpdateEmail(request.Email);

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} email updated", tenant.Id);

        return Unit.Value;
    }
}
