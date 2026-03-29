using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Note.Commands.UpdateNote;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Unit>
{
    private readonly INoteRepository _noteRepository;
    private readonly ILogger<UpdateNoteCommandHandler> _logger;

    public UpdateNoteCommandHandler(INoteRepository noteRepository, ILogger<UpdateNoteCommandHandler> logger)
    {
        _noteRepository = noteRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Note), request.NoteId);

        note.UpdateTitle(request.Title);
        note.UpdateContent(request.Content);

        await _noteRepository.UpdateAsync(note, cancellationToken);

        _logger.LogInformation("Note {NoteId} updated", note.Id);

        return Unit.Value;
    }
}
