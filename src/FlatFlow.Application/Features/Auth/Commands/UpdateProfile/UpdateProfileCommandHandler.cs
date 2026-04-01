using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
{
    private readonly IAuthService _authService;
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        IAuthService authService,
        ITenantRepository tenantRepository,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _authService = authService;
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        await _authService.UpdateUserAsync(request.UserId, request.FirstName, request.LastName, request.Email);

        var tenants = await _tenantRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        foreach (var tenant in tenants)
        {
            tenant.UpdateProfile(request.FirstName, request.LastName);
            tenant.UpdateEmail(request.Email);
            await _tenantRepository.UpdateAsync(tenant, cancellationToken);
        }

        _logger.LogInformation("Profile updated for user {UserId}, synced {Count} tenants", request.UserId, tenants.Count);

        return Unit.Value;
    }
}
