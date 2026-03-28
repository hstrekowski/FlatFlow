using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.UpdateTenantProfile;

public class UpdateTenantProfileCommandHandler : IRequestHandler<UpdateTenantProfileCommand, Unit>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<UpdateTenantProfileCommandHandler> _logger;

    public UpdateTenantProfileCommandHandler(ITenantRepository tenantRepository, ILogger<UpdateTenantProfileCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateTenantProfileCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Tenant), request.TenantId);

        tenant.UpdateProfile(request.FirstName, request.LastName);

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} profile updated", tenant.Id);

        return Unit.Value;
    }
}
