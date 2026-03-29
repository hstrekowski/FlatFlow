using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.UpdateChore;

public class UpdateChoreCommandHandler : IRequestHandler<UpdateChoreCommand, Unit>
{
    private readonly IChoreRepository _choreRepository;
    private readonly ILogger<UpdateChoreCommandHandler> _logger;

    public UpdateChoreCommandHandler(IChoreRepository choreRepository, ILogger<UpdateChoreCommandHandler> logger)
    {
        _choreRepository = choreRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateChoreCommand request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        chore.UpdateTitle(request.Title);
        chore.UpdateDescription(request.Description);
        chore.UpdateFrequency(request.Frequency);

        await _choreRepository.UpdateAsync(chore, cancellationToken);

        _logger.LogInformation("Chore {ChoreId} updated", chore.Id);

        return Unit.Value;
    }
}
