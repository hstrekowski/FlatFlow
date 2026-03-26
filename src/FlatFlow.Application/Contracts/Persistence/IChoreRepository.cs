using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Contracts.Persistence;

public interface IChoreRepository : IGenericRepository<Chore>
{
    Task<List<Chore>> GetByFlatIdAsync(Guid flatId, CancellationToken ct = default);
    Task<Chore?> GetByIdWithAssignmentsAsync(Guid id, CancellationToken ct = default);
}
