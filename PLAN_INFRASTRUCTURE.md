# Plan: Infrastructure Layer Implementation

## Context

Application layer is complete (all 7 phases done). Infrastructure layer implements repository interfaces defined in Application/Contracts/Persistence using EF Core + MSSQL. Integration tests verify correct data persistence.

**Rule: Every piece of code introduced must have corresponding integration tests in `tests/FlatFlow.Infrastructure.IntegrationTests/`.**

---

## Dependencies (NuGet packages for FlatFlow.Infrastructure)

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Configuration.Abstractions

Test project (FlatFlow.Infrastructure.IntegrationTests):
- xUnit
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory (or Testcontainers.MsSql)

---

## Implementation Phases

### Phase 1: Project Setup + DbContext

**Files to create:**
- `src/FlatFlow.Infrastructure/FlatFlow.Infrastructure.csproj`
- `src/FlatFlow.Infrastructure/InfrastructureServiceRegistration.cs`
- `src/FlatFlow.Infrastructure/Persistence/FlatFlowDbContext.cs`

**Project references:**
- FlatFlow.Infrastructure → FlatFlow.Application → FlatFlow.Domain
- FlatFlow.Infrastructure.IntegrationTests → FlatFlow.Infrastructure, FlatFlow.Application, FlatFlow.Domain

**FlatFlowDbContext:**
- DbSets: Flats, Tenants, Chores, ChoreAssignments, Payments, PaymentShares, Notes
- `OnModelCreating` → `modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())`
- Override `SaveChangesAsync` — auto-set `UpdatedAt` on modified entities via ChangeTracker

**InfrastructureServiceRegistration:**
- Register DbContext with connection string from IConfiguration
- Register GenericRepository as scoped
- Register all 5 specific repositories (IFlatRepository, ITenantRepository, IChoreRepository, IPaymentRepository, INoteRepository)

---

### Phase 2: Entity Configurations (EF Core Fluent API)

**Files to create (1 per entity):**
- `Persistence/Configurations/FlatConfiguration.cs`
- `Persistence/Configurations/TenantConfiguration.cs`
- `Persistence/Configurations/ChoreConfiguration.cs`
- `Persistence/Configurations/ChoreAssignmentConfiguration.cs`
- `Persistence/Configurations/PaymentConfiguration.cs`
- `Persistence/Configurations/PaymentShareConfiguration.cs`
- `Persistence/Configurations/NoteConfiguration.cs`

**Each configuration implements `IEntityTypeConfiguration<T>`.**

**FlatConfiguration:**
- PK: Id
- Property: Name (required, max 200)
- Property: AccessCode (required, max 20, has unique index)
- Owned type: Address (Street max 200, City max 100, ZipCode max 20, Country max 100)
- Has many: Tenants (FK: FlatId), Chores (FK: FlatId), Payments (FK: FlatId), Notes (FK: FlatId)
- Backing fields: `_tenants`, `_chores`, `_payments`, `_notes`
- Navigation access mode: Field

**TenantConfiguration:**
- PK: Id
- Properties: FirstName (required, max 100), LastName (required, max 100), Email (required, max 200), UserId (required, max 200), IsOwner
- FK: FlatId → Flat
- Index on UserId (not unique globally — user can be tenant in multiple flats)
- Composite unique index on (FlatId, UserId) — user can only be in a flat once

**ChoreConfiguration:**
- PK: Id
- Properties: Title (required, max 200), Description (max 1000), Frequency (enum stored as string or int)
- FK: FlatId → Flat
- Has many: ChoreAssignments, backing field `_choreAssignments`

**ChoreAssignmentConfiguration:**
- PK: Id
- Properties: DueDate, CompletedAt (nullable)
- FK: TenantId → Tenant, ChoreId → Chore

**PaymentConfiguration:**
- PK: Id
- Properties: Title (required, max 200), Amount (decimal precision 18,2), DueDate
- FK: FlatId → Flat, CreatedById → Tenant
- Has many: PaymentShares, backing field `_paymentShares`

**PaymentShareConfiguration:**
- PK: Id
- Properties: ShareAmount (decimal precision 18,2), Status (enum stored as string or int)
- FK: TenantId → Tenant, PaymentId → Payment

**NoteConfiguration:**
- PK: Id
- Properties: Title (required, max 200), Content (max 5000)
- FK: FlatId → Flat, AuthorId → Tenant

---

### Phase 3: Repository Implementations

**Files to create:**
- `Persistence/Repositories/GenericRepository.cs`
- `Persistence/Repositories/FlatRepository.cs`
- `Persistence/Repositories/TenantRepository.cs`
- `Persistence/Repositories/ChoreRepository.cs`
- `Persistence/Repositories/PaymentRepository.cs`
- `Persistence/Repositories/NoteRepository.cs`

**GenericRepository<T> implements IGenericRepository<T>:**
```
- GetByIdAsync(id, ct) → FindAsync(id, ct)
- GetAllAsync(ct) → ToListAsync(ct)
- AddAsync(entity, ct) → AddAsync + SaveChangesAsync → return entity
- UpdateAsync(entity, ct) → Update + SaveChangesAsync
- DeleteAsync(entity, ct) → Remove + SaveChangesAsync
```

**FlatRepository : GenericRepository<Flat>, IFlatRepository:**
```
- GetByAccessCodeAsync(accessCode, ct) → FirstOrDefaultAsync(f => f.AccessCode == accessCode)
- GetByAccessCodeWithTenantsAsync(accessCode, ct) → Include(Tenants).FirstOrDefaultAsync(f => f.AccessCode == accessCode)
- GetByTenantUserIdAsync(userId, ct) → Where(f => f.Tenants.Any(t => t.UserId == userId)).ToListAsync()
- GetByIdWithTenantsAsync(id, ct) → Include(Tenants).FirstOrDefaultAsync(f => f.Id == id)
- GetByIdWithChoresAsync(id, ct) → Include(Chores).FirstOrDefaultAsync(f => f.Id == id)
- GetByIdWithPaymentsAsync(id, ct) → Include(Payments).FirstOrDefaultAsync(f => f.Id == id)
- GetByIdWithNotesAsync(id, ct) → Include(Notes).FirstOrDefaultAsync(f => f.Id == id)
- GetByIdWithAllAsync(id, ct) → Include(Tenants).Include(Chores).ThenInclude(ChoreAssignments).Include(Payments).ThenInclude(PaymentShares).Include(Notes).FirstOrDefaultAsync(f => f.Id == id)
- GetAllPaginatedAsync(page, pageSize, ct) → Skip/Take + Count → PaginatedResult<Flat>
```

**TenantRepository : GenericRepository<Tenant>, ITenantRepository:**
```
- GetByFlatIdAsync(flatId, ct) → Where(t => t.FlatId == flatId).ToListAsync()
- GetByUserIdAsync(userId, ct) → Where(t => t.UserId == userId).ToListAsync()
```

**ChoreRepository : GenericRepository<Chore>, IChoreRepository:**
```
- GetByFlatIdAsync(flatId, ct) → Where(c => c.FlatId == flatId).ToListAsync()
- GetByIdWithAssignmentsAsync(id, ct) → Include(ChoreAssignments).FirstOrDefaultAsync(c => c.Id == id)
```

**PaymentRepository : GenericRepository<Payment>, IPaymentRepository:**
```
- GetByFlatIdPaginatedAsync(flatId, page, pageSize, ct) → Where(FlatId).Skip/Take + Count → PaginatedResult<Payment>
- GetByIdWithSharesAsync(id, ct) → Include(PaymentShares).FirstOrDefaultAsync(p => p.Id == id)
```

**NoteRepository : GenericRepository<Note>, INoteRepository:**
```
- GetByFlatIdPaginatedAsync(flatId, page, pageSize, ct) → Where(FlatId).Skip/Take + Count → PaginatedResult<Note>
```

---

### Phase 4: Integration Tests

**Project:** `tests/FlatFlow.Infrastructure.IntegrationTests/FlatFlow.Infrastructure.IntegrationTests.csproj`

**Approach:** Use InMemory database for fast tests. Each test creates a fresh DbContext.

**Test files:**
- `Persistence/Repositories/GenericRepositoryTests.cs` — CRUD basics
- `Persistence/Repositories/FlatRepositoryTests.cs` — all 9 custom methods
- `Persistence/Repositories/TenantRepositoryTests.cs` — GetByFlatIdAsync, GetByUserIdAsync
- `Persistence/Repositories/ChoreRepositoryTests.cs` — GetByFlatIdAsync, GetByIdWithAssignmentsAsync
- `Persistence/Repositories/PaymentRepositoryTests.cs` — GetByFlatIdPaginatedAsync, GetByIdWithSharesAsync
- `Persistence/Repositories/NoteRepositoryTests.cs` — GetByFlatIdPaginatedAsync
- `Persistence/FlatFlowDbContextTests.cs` — SaveChangesAsync auto-updates UpdatedAt, configurations are applied

**Test pattern:**
```csharp
public class XxxRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly XxxRepository _repository;

    // Constructor: create InMemory DbContext + repository
    // Dispose: dispose context

    // Each test: seed data → call method → assert result
}
```

---

## Execution Order

1. Phase 1 — Project setup + DbContext
2. Phase 2 — Entity Configurations
3. Phase 3 — Repository Implementations
4. Phase 4 — Integration Tests

Each phase: implement → test → verify all tests pass → move to next.

---

## Paginated Query Pattern (for repositories)

```csharp
public async Task<PaginatedResult<T>> GetXxxPaginatedAsync(int page, int pageSize, CancellationToken ct)
{
    var query = _context.Set<T>().AsQueryable(); // + optional Where filter
    var totalCount = await query.CountAsync(ct);
    var items = await query
        .OrderByDescending(e => e.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
    return new PaginatedResult<T>(items, totalCount, page, pageSize);
}
```

## Verification

```bash
dotnet test tests/FlatFlow.Infrastructure.IntegrationTests/
```
