using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Note.Queries.GetNoteById;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public GetNoteByIdQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }

    public async Task<NoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Note), request.NoteId);

        return _mapper.Map<NoteDto>(note);
    }
}
