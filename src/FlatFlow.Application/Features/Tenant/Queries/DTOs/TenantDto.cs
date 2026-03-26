namespace FlatFlow.Application.Features.Tenant.Queries.DTOs;

public record TenantDto(Guid Id, string FirstName, string LastName, string Email, bool IsOwner);
