using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Tenant.Queries.GetTenantsByFlatId;

public record GetTenantsByFlatIdQuery(Guid FlatId) : IRequest<List<TenantDto>>;
