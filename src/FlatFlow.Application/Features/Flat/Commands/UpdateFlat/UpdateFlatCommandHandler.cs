using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Flat.Commands.UpdateFlat;

public class UpdateFlatCommandHandler : IRequestHandler<UpdateFlatCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<UpdateFlatCommandHandler> _logger;

    public UpdateFlatCommandHandler(IFlatRepository flatRepository, ILogger<UpdateFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateFlatCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.UpdateName(request.Name);
        flat.UpdateAddress(new Address(request.Street, request.City, request.ZipCode, request.Country));

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Flat {FlatId} updated", flat.Id);

        return Unit.Value;
    }
}
