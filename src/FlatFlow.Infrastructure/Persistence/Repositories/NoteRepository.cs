using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class NoteRepository : GenericRepository<Note>, INoteRepository
{
    public NoteRepository(FlatFlowDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<Note>> GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Notes.Where(n => n.FlatId == flatId);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<Note>(items, totalCount, page, pageSize);
    }
}
