using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Note.Commands.AddNote;

public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<AddNoteCommandHandler> _logger;

    public AddNoteCommandHandler(IFlatRepository flatRepository, ILogger<AddNoteCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithNotesAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var note = flat.AddNote(request.Title, request.Content, request.AuthorId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Note '{NoteTitle}' added to Flat {FlatId}", request.Title, flat.Id);

        return note.Id;
    }
}
