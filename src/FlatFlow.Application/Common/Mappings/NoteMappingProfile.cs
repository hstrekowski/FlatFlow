using AutoMapper;
using FlatFlow.Application.Features.Note.Queries.DTOs;

namespace FlatFlow.Application.Common.Mappings;

public class NoteMappingProfile : Profile
{
    public NoteMappingProfile()
    {
        CreateMap<Domain.Entities.Note, NoteDto>();
    }
}
