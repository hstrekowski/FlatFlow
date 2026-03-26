using AutoMapper;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;

namespace FlatFlow.Application.Common.Mappings;

public class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<Domain.Entities.Tenant, TenantDto>();
    }
}
