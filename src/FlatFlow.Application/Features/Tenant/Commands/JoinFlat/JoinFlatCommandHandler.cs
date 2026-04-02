using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public class JoinFlatCommandHandler : IRequestHandler<JoinFlatCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly ILogger<JoinFlatCommandHandler> _logger;

    public JoinFlatCommandHandler(
        IFlatRepository flatRepository,
        ICurrentUserService currentUserService,
        IAuthService authService,
        ILogger<JoinFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _currentUserService = currentUserService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<Guid> Handle(JoinFlatCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByAccessCodeWithTenantsAsync(request.AccessCode, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.AccessCode);

        var userProfile = await _authService.GetUserAsync(_currentUserService.UserId);
        var tenant = flat.AddTenant(userProfile.FirstName, userProfile.LastName, userProfile.Email, _currentUserService.UserId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("User '{UserId}' joined Flat {FlatId} via access code", _currentUserService.UserId, flat.Id);

        return tenant.Id;
    }
}
