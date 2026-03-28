using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Tenant.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public GetTenantByIdQueryHandler(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Tenant), request.TenantId);

        return _mapper.Map<TenantDto>(tenant);
    }
}
