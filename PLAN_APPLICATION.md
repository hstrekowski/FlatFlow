# Plan: Application Layer Implementation

## Context

Domain layer is complete (272 unit tests, all passing). Application layer follows Clean Architecture + CQRS with MediatR. This layer contains use cases (Commands/Queries), repository interfaces, DTOs, validation, and pipeline behaviors.

**Rule: Every piece of code introduced must have corresponding unit tests in `tests/FlatFlow.Application.UnitTests/`.**

---

## Dependencies (NuGet packages for FlatFlow.Application)

- MediatR
- FluentValidation
- FluentValidation.DependencyInjectionExtensions
- AutoMapper
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Microsoft.Extensions.Logging.Abstractions (for ILogger<T>)

Test project (FlatFlow.Application.UnitTests):
- xUnit
- FluentAssertions
- Moq (mocking repositories and ILogger)
- AutoMapper (for testing mapping profiles)

---

## Implementation Phases

### Phase 1: Project Setup + Common Infrastructure

Create project, add NuGet packages, set up project references.

**Files to create:**
- `src/FlatFlow.Application/FlatFlow.Application.csproj`
- `src/FlatFlow.Application/ApplicationServiceRegistration.cs`
- `src/FlatFlow.Application/Common/Models/PaginatedResult.cs`
- `src/FlatFlow.Application/Common/Exceptions/NotFoundException.cs`
- `src/FlatFlow.Application/Common/Exceptions/ForbiddenException.cs`
- `src/FlatFlow.Application/Common/Behaviors/ValidationBehavior.cs`
- `src/FlatFlow.Application/Common/Behaviors/LoggingBehavior.cs`
- `src/FlatFlow.Application/Common/Behaviors/UnhandledExceptionBehavior.cs`
- `tests/FlatFlow.Application.UnitTests/FlatFlow.Application.UnitTests.csproj`

**Tests:**
- `tests/FlatFlow.Application.UnitTests/Common/Behaviors/ValidationBehaviorTests.cs`
- `tests/FlatFlow.Application.UnitTests/Common/Behaviors/LoggingBehaviorTests.cs`
- `tests/FlatFlow.Application.UnitTests/Common/Behaviors/UnhandledExceptionBehaviorTests.cs`

**Project references:**
- FlatFlow.Application → FlatFlow.Domain
- FlatFlow.Application.UnitTests → FlatFlow.Application, FlatFlow.Domain

**ApplicationServiceRegistration.cs registers:**
- MediatR (from executing assembly)
- AutoMapper (from executing assembly)
- FluentValidation validators (from executing assembly)
- ValidationBehavior as IPipelineBehavior<,>
- LoggingBehavior as IPipelineBehavior<,>
- UnhandledExceptionBehavior as IPipelineBehavior<,>

**Pipeline order:** UnhandledExceptionBehavior → LoggingBehavior → ValidationBehavior → Handler

**ValidationBehavior<TRequest, TResponse>:**
- Injects IEnumerable<IValidator<TRequest>>
- If no validators → pass through to next()
- If validators exist → run all, collect failures
- If any failures → throw FluentValidation.ValidationException(failures)
- If no failures → pass through to next()

**LoggingBehavior<TRequest, TResponse>:**
- Injects ILogger<LoggingBehavior<TRequest, TResponse>>
- Logs request name and properties at Information level before handler
- Logs request name and elapsed time at Information level after handler

**UnhandledExceptionBehavior<TRequest, TResponse>:**
- Wraps next() in try/catch
- On exception → logs Error with request name and exception details, then rethrows

**NotFoundException(string entityName, object key):**
- Message: $"{entityName} with key '{key}' was not found."

**ForbiddenException(string message):**
- Simple message-based exception.

**PaginatedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize):**
- Record type with computed HasPreviousPage and HasNextPage properties.

---

### Phase 2: Contracts (Repository Interfaces)

**Files to create:**
- `src/FlatFlow.Application/Contracts/Persistence/IGenericRepository.cs`
- `src/FlatFlow.Application/Contracts/Persistence/IFlatRepository.cs`
- `src/FlatFlow.Application/Contracts/Persistence/ITenantRepository.cs`
- `src/FlatFlow.Application/Contracts/Persistence/IChoreRepository.cs`
- `src/FlatFlow.Application/Contracts/Persistence/IPaymentRepository.cs`
- `src/FlatFlow.Application/Contracts/Persistence/INoteRepository.cs`

**IGenericRepository<T> where T : BaseEntity:**
```
Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
Task<List<T>> GetAllAsync(CancellationToken ct = default)
Task<T> AddAsync(T entity, CancellationToken ct = default)
Task UpdateAsync(T entity, CancellationToken ct = default)
Task DeleteAsync(T entity, CancellationToken ct = default)
```

**IFlatRepository : IGenericRepository<Flat>**
- GetByAccessCodeAsync(string accessCode, CancellationToken ct) → Flat?
- GetByIdWithTenantsAsync(Guid id, CancellationToken ct) → Flat?
- GetByIdWithChoresAsync(Guid id, CancellationToken ct) → Flat?
- GetByIdWithPaymentsAsync(Guid id, CancellationToken ct) → Flat?
- GetByIdWithNotesAsync(Guid id, CancellationToken ct) → Flat?
- GetByIdWithAllAsync(Guid id, CancellationToken ct) → Flat? *(loads Flat with Tenants, Chores, Payments, Notes — used by GetFlatByIdQuery)*
- GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct) → PaginatedResult<Flat>

**ITenantRepository : IGenericRepository<Tenant>**
- GetByFlatIdAsync(Guid flatId, CancellationToken ct) → List<Tenant>

**IChoreRepository : IGenericRepository<Chore>**
- GetByFlatIdAsync(Guid flatId, CancellationToken ct) → List<Chore>
- GetByIdWithAssignmentsAsync(Guid id, CancellationToken ct) → Chore?

**IPaymentRepository : IGenericRepository<Payment>**
- GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct) → PaginatedResult<Payment>
- GetByIdWithSharesAsync(Guid id, CancellationToken ct) → Payment?

**INoteRepository : IGenericRepository<Note>**
- GetByFlatIdPaginatedAsync(Guid flatId, int page, int pageSize, CancellationToken ct) → PaginatedResult<Note>

No tests needed for interfaces.

---

### Phase 3: Feature — Flat

**Mapping:**
- `src/FlatFlow.Application/Common/Mappings/FlatMappingProfile.cs`
  - Flat → FlatDto (Id, Name, City from Address.City, AccessCode)
  - Flat → FlatDetailDto (Id, Name, Address fields flattened, AccessCode, List<TenantDto>, List<ChoreDto>, List<PaymentDto>, List<NoteDto>)

**DTOs:**
- `src/FlatFlow.Application/Features/Flat/Queries/DTOs/FlatDto.cs` — Id, Name, City, AccessCode
- `src/FlatFlow.Application/Features/Flat/Queries/DTOs/FlatDetailDto.cs` — Id, Name, Street, City, ZipCode, Country, AccessCode + child DTOs

**Commands (each folder has Command.cs, CommandHandler.cs, CommandValidator.cs):**
- `CreateFlat/` — CreateFlatCommand(Name, Street, City, ZipCode, Country) → Guid
  - Handler: creates Address VO, creates Flat entity, saves via IFlatRepository.AddAsync, returns flat.Id
  - Validator: all fields NotEmpty
- `UpdateFlat/` — UpdateFlatCommand(FlatId, Name, Street, City, ZipCode, Country) → Unit
  - Handler: loads Flat via GetByIdAsync (throw NotFoundException if null), calls flat.UpdateName + flat.UpdateAddress(new Address(...)), saves via UpdateAsync
  - Validator: FlatId NotEmpty, all fields NotEmpty
- `DeleteFlat/` — DeleteFlatCommand(FlatId) → Unit
  - Handler: loads Flat (throw NotFoundException if null), calls DeleteAsync
  - Validator: FlatId NotEmpty

**Queries (each folder has Query.cs, QueryHandler.cs):**
- `GetFlatById/` — GetFlatByIdQuery(FlatId) → FlatDetailDto
  - Handler: loads Flat via IFlatRepository.GetByIdWithAllAsync (throw NotFoundException if null), maps to FlatDetailDto
- `GetFlatByAccessCode/` — GetFlatByAccessCodeQuery(AccessCode) → FlatDto
  - Handler: loads Flat by access code (throw NotFoundException if null), maps to FlatDto
- `GetAllFlats/` — GetAllFlatsQuery(Page, PageSize) → PaginatedResult<FlatDto>
  - Handler: loads PaginatedResult<Flat> via GetAllPaginatedAsync, maps Items to FlatDto, returns new PaginatedResult<FlatDto> with mapped items and same pagination metadata

**Tests:**
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/CreateFlatCommandHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/CreateFlatCommandValidatorTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/UpdateFlatCommandHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/UpdateFlatCommandValidatorTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/DeleteFlatCommandHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Commands/DeleteFlatCommandValidatorTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Queries/GetFlatByIdQueryHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Queries/GetFlatByAccessCodeQueryHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Features/Flat/Queries/GetAllFlatsQueryHandlerTests.cs`
- `tests/FlatFlow.Application.UnitTests/Common/Mappings/FlatMappingProfileTests.cs`

---

### Phase 4: Feature — Tenant

**Mapping:**
- `src/FlatFlow.Application/Common/Mappings/TenantMappingProfile.cs`
  - Tenant → TenantDto

**DTOs:**
- `src/FlatFlow.Application/Features/Tenant/Queries/DTOs/TenantDto.cs` — Id, FirstName, LastName, Email, IsOwner

**Commands:**
- `AddTenant/` — AddTenantCommand(FlatId, FirstName, LastName, Email, UserId) → Guid
  - Handler: loads Flat via IFlatRepository.GetByIdWithTenantsAsync (throw NotFoundException if null), calls flat.AddTenant(...) *(isOwner defaults to false — use PromoteTenant command separately)*, saves via IFlatRepository.UpdateAsync
  - Validator: all fields NotEmpty, Email must be valid format
- `RemoveTenant/` — RemoveTenantCommand(FlatId, TenantId) → Unit
  - Handler: loads Flat with tenants (throw NotFoundException if null), calls flat.RemoveTenant(tenantId), saves
  - Validator: FlatId NotEmpty, TenantId NotEmpty
- `UpdateTenantProfile/` — UpdateTenantProfileCommand(TenantId, FirstName, LastName) → Unit
  - Handler: loads Tenant via ITenantRepository (throw NotFoundException if null), calls tenant.UpdateProfile(...), saves
  - Validator: all fields NotEmpty
- `UpdateTenantEmail/` — UpdateTenantEmailCommand(TenantId, Email) → Unit
  - Handler: loads Tenant (throw NotFoundException if null), calls tenant.UpdateEmail(...), saves
  - Validator: TenantId NotEmpty, Email NotEmpty + valid format
- `PromoteTenant/` — PromoteTenantCommand(TenantId) → Unit
  - Handler: loads Tenant (throw NotFoundException if null), calls tenant.PromoteToOwner(), saves
  - Validator: TenantId NotEmpty
- `RevokeTenantOwnership/` — RevokeTenantOwnershipCommand(TenantId) → Unit
  - Handler: loads Tenant (throw NotFoundException if null), calls tenant.RevokeOwnership(), saves
  - Validator: TenantId NotEmpty

**Queries:**
- `GetTenantsByFlatId/` — GetTenantsByFlatIdQuery(FlatId) → List<TenantDto>
  - Handler: loads tenants via ITenantRepository.GetByFlatIdAsync, maps to List<TenantDto>
- `GetTenantById/` — GetTenantByIdQuery(TenantId) → TenantDto
  - Handler: loads tenant via ITenantRepository.GetByIdAsync (throw NotFoundException if null), maps to TenantDto

**Tests:**
- Handler + Validator tests for each command
- Query handler tests (both queries)
- TenantMappingProfile tests

---

### Phase 5: Feature — Chore

**Mapping:**
- `src/FlatFlow.Application/Common/Mappings/ChoreMappingProfile.cs`
  - Chore → ChoreDto
  - Chore → ChoreDetailDto (with assignments)
  - ChoreAssignment → ChoreAssignmentDto

**DTOs:**
- `src/FlatFlow.Application/Features/Chore/Queries/DTOs/ChoreDto.cs` — Id, Title, Description, Frequency
- `src/FlatFlow.Application/Features/Chore/Queries/DTOs/ChoreDetailDto.cs` — Id, Title, Description, Frequency, List<ChoreAssignmentDto>
- `src/FlatFlow.Application/Features/Chore/Queries/DTOs/ChoreAssignmentDto.cs` — Id, TenantId, DueDate, CompletedAt, IsCompleted

**Commands:**
- `AddChore/` — AddChoreCommand(FlatId, Title, Description, Frequency) → Guid
  - Handler: loads Flat via IFlatRepository (throw NotFoundException if null), calls flat.AddChore(...), saves via IFlatRepository.UpdateAsync
  - Validator: FlatId NotEmpty, Title NotEmpty, Frequency must be valid enum (.IsInEnum())
- `RemoveChore/` — RemoveChoreCommand(FlatId, ChoreId) → Unit
  - Handler: loads Flat with chores via GetByIdWithChoresAsync (throw NotFoundException if null), calls flat.RemoveChore(choreId), saves
  - Validator: FlatId NotEmpty, ChoreId NotEmpty
- `UpdateChore/` — UpdateChoreCommand(ChoreId, Title, Description, Frequency) → Unit
  - Handler: loads Chore via IChoreRepository (throw NotFoundException if null), calls UpdateTitle/UpdateDescription/UpdateFrequency, saves
  - Validator: ChoreId NotEmpty, Title NotEmpty, Frequency must be valid enum
- `AddChoreAssignment/` — AddChoreAssignmentCommand(ChoreId, TenantId, DueDate) → Guid
  - Handler: loads Chore via IChoreRepository.GetByIdWithAssignmentsAsync (throw NotFoundException if null), calls chore.AddAssignment(...), saves via IChoreRepository.UpdateAsync
  - Validator: ChoreId NotEmpty, TenantId NotEmpty
- `RemoveChoreAssignment/` — RemoveChoreAssignmentCommand(ChoreId, AssignmentId) → Unit
  - Handler: loads Chore with assignments via GetByIdWithAssignmentsAsync (throw NotFoundException if null), calls chore.RemoveAssignment(assignmentId), saves
  - Validator: ChoreId NotEmpty, AssignmentId NotEmpty
- `CompleteChoreAssignment/` — CompleteChoreAssignmentCommand(ChoreId, AssignmentId) → Unit
  - Handler: loads Chore with assignments (throw NotFoundException if null), finds assignment by id (throw NotFoundException if not found in collection), calls assignment.Complete(), saves via IChoreRepository.UpdateAsync
  - Validator: ChoreId NotEmpty, AssignmentId NotEmpty
- `ReopenChoreAssignment/` — ReopenChoreAssignmentCommand(ChoreId, AssignmentId) → Unit
  - Handler: loads Chore with assignments (throw NotFoundException if null), finds assignment by id (throw NotFoundException if not found in collection), calls assignment.Reopen(), saves
  - Validator: ChoreId NotEmpty, AssignmentId NotEmpty

**Queries:**
- `GetChoresByFlatId/` — GetChoresByFlatIdQuery(FlatId) → List<ChoreDto>
  - Handler: loads chores via IChoreRepository.GetByFlatIdAsync, maps
- `GetChoreById/` — GetChoreByIdQuery(ChoreId) → ChoreDetailDto
  - Handler: loads chore with assignments via GetByIdWithAssignmentsAsync (throw NotFoundException if null), maps

**Tests:**
- Handler + Validator tests for each command (7 commands × 2 = 14 test files)
- Query handler tests (2 queries)
- ChoreMappingProfile tests

---

### Phase 6: Feature — Payment

**Mapping:**
- `src/FlatFlow.Application/Common/Mappings/PaymentMappingProfile.cs`
  - Payment → PaymentDto
  - Payment → PaymentDetailDto (with shares)
  - PaymentShare → PaymentShareDto

**DTOs:**
- `src/FlatFlow.Application/Features/Payment/Queries/DTOs/PaymentDto.cs` — Id, Title, Amount, DueDate, CreatedById
- `src/FlatFlow.Application/Features/Payment/Queries/DTOs/PaymentDetailDto.cs` — Id, Title, Amount, DueDate, CreatedById, List<PaymentShareDto>
- `src/FlatFlow.Application/Features/Payment/Queries/DTOs/PaymentShareDto.cs` — Id, TenantId, ShareAmount, Status

**Commands:**
- `AddPayment/` — AddPaymentCommand(FlatId, Title, Amount, DueDate, CreatedById) → Guid
  - Handler: loads Flat (throw NotFoundException if null), calls flat.AddPayment(...), saves via IFlatRepository.UpdateAsync
  - Validator: FlatId NotEmpty, Title NotEmpty, Amount > 0 (.GreaterThan(0)), CreatedById NotEmpty
- `RemovePayment/` — RemovePaymentCommand(FlatId, PaymentId) → Unit
  - Handler: loads Flat with payments via GetByIdWithPaymentsAsync (throw NotFoundException if null), calls flat.RemovePayment(paymentId), saves
  - Validator: FlatId NotEmpty, PaymentId NotEmpty
- `UpdatePayment/` — UpdatePaymentCommand(PaymentId, Title, Amount, DueDate) → Unit
  - Handler: loads Payment via IPaymentRepository (throw NotFoundException if null), calls UpdateTitle/UpdateAmount/UpdateDueDate, saves
  - Validator: PaymentId NotEmpty, Title NotEmpty, Amount > 0
- `AddPaymentShare/` — AddPaymentShareCommand(PaymentId, TenantId, ShareAmount) → Guid
  - Handler: loads Payment via IPaymentRepository.GetByIdWithSharesAsync (throw NotFoundException if null), calls payment.AddShare(...), saves via IPaymentRepository.UpdateAsync
  - Validator: PaymentId NotEmpty, TenantId NotEmpty, ShareAmount > 0
- `RemovePaymentShare/` — RemovePaymentShareCommand(PaymentId, ShareId) → Unit
  - Handler: loads Payment with shares via GetByIdWithSharesAsync (throw NotFoundException if null), calls payment.RemoveShare(shareId), saves
  - Validator: PaymentId NotEmpty, ShareId NotEmpty
- `MarkShareAsPaid/` — MarkShareAsPaidCommand(PaymentId, ShareId) → Unit
  - Handler: loads Payment with shares (throw NotFoundException if null), finds share by id (throw NotFoundException if not found in collection), calls share.MarkAsPaid(), saves via IPaymentRepository.UpdateAsync
  - Validator: PaymentId NotEmpty, ShareId NotEmpty
- `MarkShareAsPartial/` — MarkShareAsPartialCommand(PaymentId, ShareId) → Unit
  - Handler: loads Payment with shares (throw NotFoundException if null), finds share by id (throw NotFoundException if not found in collection), calls share.MarkAsPartial(), saves
  - Validator: PaymentId NotEmpty, ShareId NotEmpty

**Queries:**
- `GetPaymentsByFlatId/` — GetPaymentsByFlatIdQuery(FlatId, Page, PageSize) → PaginatedResult<PaymentDto>
  - Handler: loads PaginatedResult<Payment> via IPaymentRepository.GetByFlatIdPaginatedAsync, maps Items to PaymentDto, returns new PaginatedResult<PaymentDto>
- `GetPaymentById/` — GetPaymentByIdQuery(PaymentId) → PaymentDetailDto
  - Handler: loads payment with shares via GetByIdWithSharesAsync (throw NotFoundException if null), maps

**Tests:**
- Handler + Validator tests for each command (7 commands × 2 = 14 test files)
- Query handler tests (2 queries)
- PaymentMappingProfile tests

---

### Phase 7: Feature — Note

**Mapping:**
- `src/FlatFlow.Application/Common/Mappings/NoteMappingProfile.cs`
  - Note → NoteDto

**DTOs:**
- `src/FlatFlow.Application/Features/Note/Queries/DTOs/NoteDto.cs` — Id, Title, Content, AuthorId, CreatedAt

**Commands:**
- `AddNote/` — AddNoteCommand(FlatId, Title, Content, AuthorId) → Guid
  - Handler: loads Flat (throw NotFoundException if null), calls flat.AddNote(...), saves via IFlatRepository.UpdateAsync
  - Validator: FlatId NotEmpty, Title NotEmpty, AuthorId NotEmpty
- `RemoveNote/` — RemoveNoteCommand(FlatId, NoteId) → Unit
  - Handler: loads Flat with notes via GetByIdWithNotesAsync (throw NotFoundException if null), calls flat.RemoveNote(noteId), saves
  - Validator: FlatId NotEmpty, NoteId NotEmpty
- `UpdateNote/` — UpdateNoteCommand(NoteId, Title, Content) → Unit
  - Handler: loads Note via INoteRepository (throw NotFoundException if null), calls UpdateTitle/UpdateContent, saves
  - Validator: NoteId NotEmpty, Title NotEmpty

**Queries:**
- `GetNotesByFlatId/` — GetNotesByFlatIdQuery(FlatId, Page, PageSize) → PaginatedResult<NoteDto>
  - Handler: loads PaginatedResult<Note> via INoteRepository.GetByFlatIdPaginatedAsync, maps Items to NoteDto, returns new PaginatedResult<NoteDto>

**Tests:**
- Handler + Validator tests for each command (3 commands × 2 = 6 test files)
- Query handler tests (1 query)
- NoteMappingProfile tests

---

## Execution Order

1. Phase 1 — Project setup + Common (Behaviors, Exceptions, Models) + tests
2. Phase 2 — Contracts (repository interfaces, no tests needed)
3. Phase 3 — Flat feature (Commands, Queries, DTOs, Mappings) + tests
4. Phase 4 — Tenant feature + tests
5. Phase 5 — Chore feature + tests
6. Phase 6 — Payment feature + tests
7. Phase 7 — Note feature + tests

Each phase: implement → test → verify all tests pass → move to next.

---

## Handler Pattern (every command handler follows this)

```csharp
public class XxxCommandHandler : IRequestHandler<XxxCommand, ReturnType>
{
    private readonly IXxxRepository _repository;
    private readonly ILogger<XxxCommandHandler> _logger;

    // 1. Load aggregate from repository (throw NotFoundException if null)
    // 2. Call domain method (domain validates and throws DomainException/DomainValidationException if invalid)
    // 3. Save via repository
    // 4. Log important business event at Information level
    // 5. Return result (Guid for creates, Unit for updates/deletes)
}
```

## Query Handler Pattern

```csharp
public class XxxQueryHandler : IRequestHandler<XxxQuery, ReturnType>
{
    private readonly IXxxRepository _repository;
    private readonly IMapper _mapper;

    // 1. Load data from repository
    // 2. Throw NotFoundException if single entity query returns null
    // 3. Map to DTO via AutoMapper
    // 4. Return DTO
}
```

## Paginated Query Mapping Pattern

```csharp
// Repo returns PaginatedResult<Entity>, handler maps to PaginatedResult<Dto>
var result = await _repository.GetXxxPaginatedAsync(page, pageSize, ct);
var dtos = _mapper.Map<List<XxxDto>>(result.Items);
return new PaginatedResult<XxxDto>(dtos, result.TotalCount, result.Page, result.PageSize);
```

## Test Pattern (for command handlers)

```csharp
public class XxxCommandHandlerTests
{
    private readonly Mock<IXxxRepository> _repositoryMock;
    private readonly Mock<ILogger<XxxCommandHandler>> _loggerMock;
    private readonly XxxCommandHandler _handler;

    // Constructor: setup mocks + create handler

    // Happy path: verify domain method called, verify repository.UpdateAsync called, verify return value
    // Not found: repository returns null → assert NotFoundException thrown
    // Domain validation failure: domain throws → assert exception propagates
}
```

## Test Pattern (for child entity commands — Complete/Reopen/MarkAs)

```csharp
// Additional test case: parent exists but child not found in collection
// → assert NotFoundException("ChoreAssignment", assignmentId)
```

## Verification

```bash
dotnet test tests/FlatFlow.Application.UnitTests/
```
