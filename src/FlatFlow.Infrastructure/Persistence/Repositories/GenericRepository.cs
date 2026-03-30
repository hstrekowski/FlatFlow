using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly FlatFlowDbContext _context;

    public GenericRepository(FlatFlowDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<T>().FindAsync([id], ct);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<T>().ToListAsync(ct);
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}
