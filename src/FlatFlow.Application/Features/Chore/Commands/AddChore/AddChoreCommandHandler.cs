using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Chore.Commands.AddChore;

public class AddChoreCommandHandler : IRequestHandler<AddChoreCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<AddChoreCommandHandler> _logger;

    public AddChoreCommandHandler(IFlatRepository flatRepository, ILogger<AddChoreCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddChoreCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithChoresAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var chore = flat.AddChore(request.Title, request.Description, request.Frequency, request.CreatedById);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Chore '{ChoreTitle}' added to Flat {FlatId}", request.Title, flat.Id);

        return chore.Id;
    }
}
