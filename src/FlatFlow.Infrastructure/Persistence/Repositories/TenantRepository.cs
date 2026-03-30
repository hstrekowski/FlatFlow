using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(FlatFlowDbContext context) : base(context)
    {
    }

    public async Task<List<Tenant>> GetByFlatIdAsync(Guid flatId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .Where(t => t.FlatId == flatId)
            .ToListAsync(ct);
    }

    public async Task<List<Tenant>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
    }
}
