using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.Persistence.Repositories;

public class ChoreRepository : GenericRepository<Chore>, IChoreRepository
{
    public ChoreRepository(FlatFlowDbContext context) : base(context)
    {
    }

    public async Task<List<Chore>> GetByFlatIdAsync(Guid flatId, CancellationToken ct = default)
    {
        return await _context.Chores
            .Where(c => c.FlatId == flatId)
            .ToListAsync(ct);
    }

    public async Task<Chore?> GetByIdWithAssignmentsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Chores
            .Include(c => c.ChoreAssignments)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
