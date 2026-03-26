using FlatFlow.Application.Common.Models;
using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Contracts.Persistence;

public interface IFlatRepository : IGenericRepository<Flat>
{
    Task<Flat?> GetByAccessCodeAsync(string accessCode, CancellationToken ct = default);
    Task<Flat?> GetByIdWithTenantsAsync(Guid id, CancellationToken ct = default);
    Task<Flat?> GetByIdWithChoresAsync(Guid id, CancellationToken ct = default);
    Task<Flat?> GetByIdWithPaymentsAsync(Guid id, CancellationToken ct = default);
    Task<Flat?> GetByIdWithNotesAsync(Guid id, CancellationToken ct = default);
    Task<Flat?> GetByIdWithAllAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<Flat>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default);
}
