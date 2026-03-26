using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Flat.Commands.DeleteFlat;

public class DeleteFlatCommandHandler : IRequestHandler<DeleteFlatCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<DeleteFlatCommandHandler> _logger;

    public DeleteFlatCommandHandler(IFlatRepository flatRepository, ILogger<DeleteFlatCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteFlatCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        await _flatRepository.DeleteAsync(flat, cancellationToken);

        _logger.LogInformation("Flat {FlatId} deleted", flat.Id);

        return Unit.Value;
    }
}
