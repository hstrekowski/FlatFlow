using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Flat.Commands.RefreshAccessCode;

public class RefreshAccessCodeCommandHandler : IRequestHandler<RefreshAccessCodeCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<RefreshAccessCodeCommandHandler> _logger;

    public RefreshAccessCodeCommandHandler(IFlatRepository flatRepository, ILogger<RefreshAccessCodeCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RefreshAccessCodeCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.RefreshAccessCode();

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Access code refreshed for Flat {FlatId}", flat.Id);

        return Unit.Value;
    }
}
