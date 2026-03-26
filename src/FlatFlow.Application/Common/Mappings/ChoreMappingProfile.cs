using AutoMapper;
using FlatFlow.Application.Features.Chore.Queries.DTOs;

namespace FlatFlow.Application.Common.Mappings;

public class ChoreMappingProfile : Profile
{
    public ChoreMappingProfile()
    {
        CreateMap<Domain.Entities.Chore, ChoreDto>();
    }
}
