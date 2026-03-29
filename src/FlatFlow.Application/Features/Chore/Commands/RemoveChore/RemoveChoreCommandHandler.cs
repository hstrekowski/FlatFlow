using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChore;

public class RemoveChoreCommandHandler : IRequestHandler<RemoveChoreCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<RemoveChoreCommandHandler> _logger;

    public RemoveChoreCommandHandler(IFlatRepository flatRepository, ILogger<RemoveChoreCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveChoreCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithChoresAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.RemoveChore(request.ChoreId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Chore {ChoreId} removed from Flat {FlatId}", request.ChoreId, flat.Id);

        return Unit.Value;
    }
}
