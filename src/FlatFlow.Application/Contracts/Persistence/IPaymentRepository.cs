using FlatFlow.Application.Common.Models;
using FlatFlow.Domain.Entities;

namespace FlatFlow.Application.Contracts.Persistence;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<PaginatedResult<Payment>> GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct = default);
    Task<Payment?> GetByIdWithSharesAsync(Guid id, CancellationToken ct = default);
}
