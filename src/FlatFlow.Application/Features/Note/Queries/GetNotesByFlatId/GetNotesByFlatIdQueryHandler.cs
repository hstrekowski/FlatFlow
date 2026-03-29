using AutoMapper;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Note.Queries.GetNotesByFlatId;

public class GetNotesByFlatIdQueryHandler : IRequestHandler<GetNotesByFlatIdQuery, PaginatedResult<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public GetNotesByFlatIdQueryHandler(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<NoteDto>> Handle(GetNotesByFlatIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _noteRepository.GetByFlatIdPaginatedAsync(request.FlatId, request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<NoteDto>>(result.Items);

        return new PaginatedResult<NoteDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
