using FlatFlow.Application.Features.Note.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Note.Queries.GetNoteById;

public record GetNoteByIdQuery(Guid NoteId) : IRequest<NoteDto>;
