using AutoMapper;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Common.Mappings;

public class FlatMappingProfile : Profile
{
    public FlatMappingProfile()
    {
        CreateMap<Flat, FlatDto>()
            .ForCtorParam("City", opt => opt.MapFrom(s => s.Address.City));

        CreateMap<Flat, FlatDetailDto>()
            .ForCtorParam("Street", opt => opt.MapFrom(s => s.Address.Street))
            .ForCtorParam("City", opt => opt.MapFrom(s => s.Address.City))
            .ForCtorParam("ZipCode", opt => opt.MapFrom(s => s.Address.ZipCode))
            .ForCtorParam("Country", opt => opt.MapFrom(s => s.Address.Country));
    }
}
