# Plan: Aggregate Root Methods + Custom Domain Exceptions

## Context
The domain layer has entities with proper validation and encapsulation, but no aggregate root methods for managing child entities. When the Application layer is built, handlers would have to manually create children and wire up foreign keys, bypassing domain logic. Additionally, all exceptions are generic (`ArgumentException`, `InvalidOperationException`), which will make it hard to return proper HTTP responses from the API layer.

---

## Phase 1: Aggregate Root Methods

Add methods to parent entities so they control creation and removal of their children.

### Flat.cs — add:
- `AddTenant(string firstName, string lastName, string email, string userId, bool isOwner = false)` → creates & adds Tenant
- `RemoveTenant(Guid tenantId)` → finds & removes Tenant (throws if not found)
- `AddChore(string title, string description, ChoreFrequency frequency)` → creates & adds Chore
- `RemoveChore(Guid choreId)` → finds & removes Chore (throws if not found)
- `AddNote(string title, string content, Guid authorId)` → creates & adds Note
- `RemoveNote(Guid noteId)` → finds & removes Note (throws if not found)
- `AddPayment(string title, decimal amount, DateTime dueDate, Guid createdById)` → creates & adds Payment
- `RemovePayment(Guid paymentId)` → finds & removes Payment (throws if not found)

### Payment.cs — add:
- `AddShare(Guid tenantId, decimal shareAmount)` → creates & adds PaymentShare
- `RemoveShare(Guid shareId)` → finds & removes PaymentShare (throws if not found)

### Chore.cs — add:
- `AddAssignment(Guid tenantId, DateTime dueDate)` → creates & adds ChoreAssignment
- `RemoveAssignment(Guid assignmentId)` → finds & removes ChoreAssignment (throws if not found)

### Note on updating children:
Child entities already have their own update methods (e.g., `Tenant.UpdateProfile()`, `PaymentShare.MarkAsPaid()`).
No wrapper methods needed on the parent — the Application layer loads the aggregate, finds the child, and calls the child's method directly.

### Files to modify:
- `src/FlatFlow.Domain/Entities/Flat.cs`
- `src/FlatFlow.Domain/Entities/Payment.cs`
- `src/FlatFlow.Domain/Entities/Chore.cs`

---

## Phase 2: Custom Domain Exceptions

### New folder: `src/FlatFlow.Domain/Exceptions/`

### New files:
- `DomainException.cs` — base exception for domain rule violations (e.g., "Tenant is already an owner")
- `DomainValidationException.cs` — for input validation failures (e.g., "First name cannot be empty"), includes `PropertyName`

```
DomainException : Exception
└── DomainValidationException : DomainException  (adds PropertyName)
```

### Update all entities to use new exceptions:
- `ArgumentException` → `DomainValidationException` (constructor/method input validation)
- `InvalidOperationException` → `DomainException` (state violations)
- `ArgumentNullException` → `DomainValidationException` (null checks like Address in Flat)

### Files to modify:
- `src/FlatFlow.Domain/Entities/Tenant.cs`
- `src/FlatFlow.Domain/Entities/Flat.cs`
- `src/FlatFlow.Domain/Entities/Payment.cs`
- `src/FlatFlow.Domain/Entities/PaymentShare.cs`
- `src/FlatFlow.Domain/Entities/Chore.cs`
- `src/FlatFlow.Domain/Entities/ChoreAssignment.cs`
- `src/FlatFlow.Domain/Entities/Note.cs`
- `src/FlatFlow.Domain/ValueObjects/Address.cs`

---

## Phase 3: Update Existing Unit Tests

Update all existing test files to expect `DomainValidationException` / `DomainException` instead of `ArgumentException` / `InvalidOperationException`.

### Files to modify:
- `tests/FlatFlow.Domain.UnitTests/TenantTests.cs`
- `tests/FlatFlow.Domain.UnitTests/FlatTests.cs`
- `tests/FlatFlow.Domain.UnitTests/PaymentTests.cs`
- `tests/FlatFlow.Domain.UnitTests/PaymentShareTests.cs`
- `tests/FlatFlow.Domain.UnitTests/ChoreTests.cs`
- `tests/FlatFlow.Domain.UnitTests/ChoreAssignmentTests.cs`
- `tests/FlatFlow.Domain.UnitTests/NoteTests.cs`
- `tests/FlatFlow.Domain.UnitTests/AddressTests.cs`

---

## Phase 4: New Unit Tests for Aggregate Root Methods

Add tests for all new aggregate methods in the existing test files:

### FlatTests.cs — add tests for:
- `AddTenant` — happy path (returns tenant, adds to collection, sets FlatId)
- `AddTenant` — validation (invalid name/email/userId throws)
- `RemoveTenant` — happy path (removes from collection)
- `RemoveTenant` — throws when tenant not found
- `AddChore` — happy path + validation
- `RemoveChore` — happy path + not found
- `AddNote` — happy path + validation (empty authorId)
- `RemoveNote` — happy path + not found
- `AddPayment` — happy path + validation (invalid amount, empty createdById)
- `RemovePayment` — happy path + not found

### PaymentTests.cs — add tests for:
- `AddShare` — happy path (returns share, adds to collection, sets PaymentId)
- `AddShare` — validation (empty tenantId, invalid amount)
- `RemoveShare` — happy path + not found

### ChoreTests.cs — add tests for:
- `AddAssignment` — happy path (returns assignment, adds to collection, sets ChoreId)
- `AddAssignment` — validation (empty tenantId)
- `RemoveAssignment` — happy path + not found

---

## Execution Order
1. Create `Exceptions/DomainException.cs` and `Exceptions/DomainValidationException.cs`
2. Add aggregate root methods to `Flat.cs`, `Payment.cs`, `Chore.cs`
3. Replace all generic exceptions across all entities + Address
4. Update all existing tests for new exception types
5. Add new tests for aggregate root methods
6. Run `dotnet test` to verify everything passes

---

## Verification
```bash
dotnet test tests/FlatFlow.Domain.UnitTests/
```
All existing tests pass (with updated exception types) + all new aggregate method tests pass.
