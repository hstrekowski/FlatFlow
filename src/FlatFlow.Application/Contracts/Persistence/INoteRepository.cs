using FlatFlow.Application.Common.Models;
using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Contracts.Persistence;

public interface INoteRepository : IGenericRepository<Note>
{
    Task<PaginatedResult<Note>> GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct = default);
}
