using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Contracts.Persistence;

public interface ITenantRepository : IGenericRepository<Tenant>
{
    Task<List<Tenant>> GetByFlatIdAsync(Guid flatId, CancellationToken ct = default);
    Task<List<Tenant>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Tenant?> GetByUserIdAndFlatIdAsync(string userId, Guid flatId, CancellationToken ct = default);
}
