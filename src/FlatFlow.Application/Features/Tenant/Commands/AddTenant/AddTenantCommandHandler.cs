using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.AddTenant;

public class AddTenantCommandHandler : IRequestHandler<AddTenantCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<AddTenantCommandHandler> _logger;

    public AddTenantCommandHandler(IFlatRepository flatRepository, ILogger<AddTenantCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddTenantCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithTenantsAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var tenant = flat.AddTenant(request.FirstName, request.LastName, request.Email, request.UserId, request.IsOwner);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Tenant '{TenantEmail}' added to Flat {FlatId}", request.Email, flat.Id);

        return tenant.Id;
    }
}
