using AutoMapper;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Tenant.Queries.GetTenantsByFlatId;

public class GetTenantsByFlatIdQueryHandler : IRequestHandler<GetTenantsByFlatIdQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public GetTenantsByFlatIdQueryHandler(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<List<TenantDto>> Handle(GetTenantsByFlatIdQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetByFlatIdAsync(request.FlatId, cancellationToken);

        return _mapper.Map<List<TenantDto>>(tenants);
    }
}
