using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Note.Commands.RemoveNote;

public class RemoveNoteCommandHandler : IRequestHandler<RemoveNoteCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<RemoveNoteCommandHandler> _logger;

    public RemoveNoteCommandHandler(IFlatRepository flatRepository, ILogger<RemoveNoteCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithNotesAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.RemoveNote(request.NoteId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Note {NoteId} removed from Flat {FlatId}", request.NoteId, flat.Id);

        return Unit.Value;
    }
}
