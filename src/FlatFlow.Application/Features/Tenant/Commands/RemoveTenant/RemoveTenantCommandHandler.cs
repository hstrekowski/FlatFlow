using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;

public class RemoveTenantCommandHandler : IRequestHandler<RemoveTenantCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<RemoveTenantCommandHandler> _logger;

    public RemoveTenantCommandHandler(IFlatRepository flatRepository, ILogger<RemoveTenantCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveTenantCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithTenantsAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.RemoveTenant(request.TenantId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} removed from Flat {FlatId}", request.TenantId, flat.Id);

        return Unit.Value;
    }
}
