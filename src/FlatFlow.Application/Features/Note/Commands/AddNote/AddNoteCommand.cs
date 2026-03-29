using MediatR;

namespace FlatFlow.Application.Features.Note.Commands.AddNote;

public record AddNoteCommand(
    Guid FlatId,
    string Title,
    string Content,
    Guid AuthorId) : IRequest<Guid>;
