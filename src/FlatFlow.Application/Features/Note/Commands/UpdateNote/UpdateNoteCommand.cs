using MediatR;

namespace FlatFlow.Application.Features.Note.Commands.UpdateNote;

public record UpdateNoteCommand(
    Guid NoteId,
    string Title,
    string Content) : IRequest<Unit>;
