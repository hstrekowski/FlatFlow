using AutoMapper;
using FlatFlow.Application.Features.Chore.Queries.DTOs;

namespace FlatFlow.Application.Common.Mappings;

public class ChoreMappingProfile : Profile
{
    public ChoreMappingProfile()
    {
        CreateMap<Domain.Entities.Chore, ChoreDto>();
        CreateMap<Domain.Entities.Chore, ChoreDetailDto>()
            .ForCtorParam("Assignments", opt => opt.MapFrom(src => src.ChoreAssignments));
        CreateMap<Domain.Entities.ChoreAssignment, ChoreAssignmentDto>();
    }
}
