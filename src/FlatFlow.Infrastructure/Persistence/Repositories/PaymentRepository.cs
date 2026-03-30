using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(FlatFlowDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<Payment>> GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Payments.Where(p => p.FlatId == flatId);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<Payment>(items, totalCount, page, pageSize);
    }

    public async Task<Payment?> GetByIdWithSharesAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Payments
            .Include(p => p.PaymentShares)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
