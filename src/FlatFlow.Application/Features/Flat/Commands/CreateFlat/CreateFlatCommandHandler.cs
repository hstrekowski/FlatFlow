using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Flat.Commands.CreateFlat;

public class CreateFlatCommandHandler : IRequestHandler<CreateFlatCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<CreateFlatCommandHandler> _logger;

    public CreateFlatCommandHandler(IFlatRepository flatRepository, ILogger<CreateFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateFlatCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.ZipCode, request.Country);
        var flat = new Domain.Entities.Flat(request.Name, address);

        await _flatRepository.AddAsync(flat, cancellationToken);

        _logger.LogInformation("Flat '{FlatName}' created with ID {FlatId}", flat.Name, flat.Id);

        return flat.Id;
    }
}
