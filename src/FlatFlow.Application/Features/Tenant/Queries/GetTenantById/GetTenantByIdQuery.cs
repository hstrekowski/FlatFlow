using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Tenant.Queries.GetTenantById;

public record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto>;
