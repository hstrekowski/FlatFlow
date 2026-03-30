using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class FlatRepository : GenericRepository<Flat>, IFlatRepository
{
    public FlatRepository(FlatFlowDbContext context) : base(context)
    {
    }

    public async Task<Flat?> GetByAccessCodeAsync(string accessCode, CancellationToken ct = default)
    {
        return await _context.Flats
            .FirstOrDefaultAsync(f => f.AccessCode == accessCode, ct);
    }

    public async Task<Flat?> GetByAccessCodeWithTenantsAsync(string accessCode, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Tenants)
            .FirstOrDefaultAsync(f => f.AccessCode == accessCode, ct);
    }

    public async Task<List<Flat>> GetByTenantUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Flats
            .Where(f => f.Tenants.Any(t => t.UserId == userId))
            .ToListAsync(ct);
    }

    public async Task<Flat?> GetByIdWithTenantsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Tenants)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Flat?> GetByIdWithChoresAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Chores)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Flat?> GetByIdWithPaymentsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Payments)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Flat?> GetByIdWithNotesAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Notes)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Flat?> GetByIdWithAllAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Flats
            .Include(f => f.Tenants)
            .Include(f => f.Chores)
                .ThenInclude(c => c.ChoreAssignments)
            .Include(f => f.Payments)
                .ThenInclude(p => p.PaymentShares)
            .Include(f => f.Notes)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<PaginatedResult<Flat>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Flats.AsQueryable();
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<Flat>(items, totalCount, page, pageSize);
    }
}
