using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Note.Queries.GetNotesByFlatId;

public record GetNotesByFlatIdQuery(Guid FlatId, int Page, int PageSize) : IRequest<PaginatedResult<NoteDto>>;
