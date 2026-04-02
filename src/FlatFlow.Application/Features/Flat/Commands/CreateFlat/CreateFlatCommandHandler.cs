using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Flat.Commands.CreateFlat;

public class CreateFlatCommandHandler : IRequestHandler<CreateFlatCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly ILogger<CreateFlatCommandHandler> _logger;

    public CreateFlatCommandHandler(
        IFlatRepository flatRepository,
        ICurrentUserService currentUserService,
        IAuthService authService,
        ILogger<CreateFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _currentUserService = currentUserService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateFlatCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.ZipCode, request.Country);
        var flat = new Domain.Entities.Flat(request.Name, address);

        var userProfile = await _authService.GetUserAsync(_currentUserService.UserId);
        flat.AddTenant(userProfile.FirstName, userProfile.LastName, userProfile.Email, _currentUserService.UserId, isOwner: true);

        await _flatRepository.AddAsync(flat, cancellationToken);

        _logger.LogInformation("Flat '{FlatName}' created with ID {FlatId} by user {UserId}", flat.Name, flat.Id, _currentUserService.UserId);

        return flat.Id;
    }
}
