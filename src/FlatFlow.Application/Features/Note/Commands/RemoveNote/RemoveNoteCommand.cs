using MediatR;

namespace FlatFlow.Application.Features.Note.Commands.RemoveNote;

public record RemoveNoteCommand(Guid FlatId, Guid NoteId) : IRequest<Unit>;
