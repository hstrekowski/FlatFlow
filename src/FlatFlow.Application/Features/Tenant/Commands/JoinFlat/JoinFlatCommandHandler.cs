using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public class JoinFlatCommandHandler : IRequestHandler<JoinFlatCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<JoinFlatCommandHandler> _logger;

    public JoinFlatCommandHandler(IFlatRepository flatRepository, ILogger<JoinFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(JoinFlatCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByAccessCodeWithTenantsAsync(request.AccessCode, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.AccessCode);

        var tenant = flat.AddTenant(request.FirstName, request.LastName, request.Email, request.UserId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("User '{UserId}' joined Flat {FlatId} via access code", request.UserId, flat.Id);

        return tenant.Id;
    }
}
