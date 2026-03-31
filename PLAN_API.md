# Plan: API Layer Implementation

## Context

Application layer (35 commands, 16 queries) and Infrastructure layer are complete. API layer is a thin layer: controllers send requests to MediatR, middleware maps exceptions to HTTP status codes.

**No authentication** — will be handled separately after API is done.

---

## Phase 1: Program.cs + Global Exception Middleware

**Files:**
- `Middleware/GlobalExceptionHandler.cs`
- `Program.cs` — update

**Exception mapping (ORDER MATTERS — check subclasses before base classes):**

| Order | Exception | HTTP Status | Response |
|---|---|---|---|
| 1 | `FluentValidation.ValidationException` | 400 Bad Request | `{ errors: { propertyName: ["msg"] } }` |
| 2 | `DomainValidationException` | 400 Bad Request | `{ errors: { propertyName: ["msg"] } }` |
| 3 | `NotFoundException` | 404 Not Found | `{ message: "..." }` |
| 4 | `ForbiddenException` | 403 Forbidden | `{ message: "..." }` |
| 5 | `DomainException` | 422 Unprocessable Entity | `{ message: "..." }` |
| 6 | Unhandled | 500 Internal Server Error | `{ message: "An unexpected error occurred." }` |

**IMPORTANT:** DomainValidationException inherits from DomainException — must be checked BEFORE DomainException, otherwise all domain validation errors would return 422 instead of 400.

**Program.cs:**
- AddApplicationServices()
- AddInfrastructureServices(configuration)
- AddControllers()
- UseExceptionHandler (GlobalExceptionHandler)
- MapControllers()

---

## Phase 2: FlatsController

**Route:** `api/flats`

| Endpoint | Method | MediatR Request | Response |
|---|---|---|---|
| `POST /` | Create | `CreateFlatCommand` | 201 + Guid |
| `GET /` | GetAll | `GetAllFlatsQuery` | 200 + PaginatedResult |
| `GET /{id}` | GetById | `GetFlatByIdQuery` | 200 + FlatDetailDto |
| `GET /by-access-code/{code}` | GetByAccessCode | `GetFlatByAccessCodeQuery` | 200 + FlatDto |
| `GET /by-user/{userId}` | GetByUserId | `GetFlatsByUserIdQuery` | 200 + List FlatDto |
| `PUT /{id}` | Update | `UpdateFlatCommand` | 204 |
| `DELETE /{id}` | Delete | `DeleteFlatCommand` | 204 |
| `POST /{id}/refresh-access-code` | RefreshCode | `RefreshAccessCodeCommand` | 204 |

---

## Phase 3: TenantsController

**Route:** `api/flats/{flatId}/tenants`

| Endpoint | Method | MediatR Request | Response |
|---|---|---|---|
| `POST /` | Add | `AddTenantCommand` | 201 + Guid |
| `POST /join` | Join | `JoinFlatCommand` | 201 + Guid |
| `GET /` | GetByFlatId | `GetTenantsByFlatIdQuery` | 200 + List TenantDto |
| `GET /{tenantId}` | GetById | `GetTenantByIdQuery` | 200 + TenantDto |
| `PUT /{tenantId}/profile` | UpdateProfile | `UpdateTenantProfileCommand` | 204 |
| `PUT /{tenantId}/email` | UpdateEmail | `UpdateTenantEmailCommand` | 204 |
| `POST /{tenantId}/promote` | Promote | `PromoteTenantCommand` | 204 |
| `POST /{tenantId}/revoke-ownership` | Revoke | `RevokeTenantOwnershipCommand` | 204 |
| `DELETE /{tenantId}` | Remove | `RemoveTenantCommand` | 204 |

---

## Phase 4: ChoresController

**Route:** `api/flats/{flatId}/chores`

| Endpoint | Method | MediatR Request | Response |
|---|---|---|---|
| `POST /` | Add | `AddChoreCommand` | 201 + Guid |
| `GET /` | GetByFlatId | `GetChoresByFlatIdQuery` | 200 + List ChoreDto |
| `GET /{choreId}` | GetById | `GetChoreByIdQuery` | 200 + ChoreDetailDto |
| `PUT /{choreId}` | Update | `UpdateChoreCommand` | 204 |
| `DELETE /{choreId}` | Remove | `RemoveChoreCommand` | 204 |
| `POST /{choreId}/assignments` | AddAssignment | `AddChoreAssignmentCommand` | 201 + Guid |
| `DELETE /{choreId}/assignments/{assignmentId}` | RemoveAssignment | `RemoveChoreAssignmentCommand` | 204 |
| `POST /{choreId}/assignments/{assignmentId}/complete` | Complete | `CompleteChoreAssignmentCommand` | 204 |
| `POST /{choreId}/assignments/{assignmentId}/reopen` | Reopen | `ReopenChoreAssignmentCommand` | 204 |

---

## Phase 5: PaymentsController

**Route:** `api/flats/{flatId}/payments`

| Endpoint | Method | MediatR Request | Response |
|---|---|---|---|
| `POST /` | Add | `AddPaymentCommand` | 201 + Guid |
| `GET /` | GetByFlatId | `GetPaymentsByFlatIdQuery` | 200 + PaginatedResult |
| `GET /{paymentId}` | GetById | `GetPaymentByIdQuery` | 200 + PaymentDetailDto |
| `PUT /{paymentId}` | Update | `UpdatePaymentCommand` | 204 |
| `DELETE /{paymentId}` | Remove | `RemovePaymentCommand` | 204 |
| `POST /{paymentId}/shares` | AddShare | `AddPaymentShareCommand` | 201 + Guid |
| `DELETE /{paymentId}/shares/{shareId}` | RemoveShare | `RemovePaymentShareCommand` | 204 |
| `POST /{paymentId}/shares/{shareId}/mark-paid` | MarkPaid | `MarkShareAsPaidCommand` | 204 |
| `POST /{paymentId}/shares/{shareId}/mark-partial` | MarkPartial | `MarkShareAsPartialCommand` | 204 |

---

## Phase 6: NotesController

**Route:** `api/flats/{flatId}/notes`

| Endpoint | Method | MediatR Request | Response |
|---|---|---|---|
| `POST /` | Add | `AddNoteCommand` | 201 + Guid |
| `GET /` | GetByFlatId | `GetNotesByFlatIdQuery` | 200 + PaginatedResult |
| `GET /{noteId}` | GetById | `GetNoteByIdQuery` | 200 + NoteDto |
| `PUT /{noteId}` | Update | `UpdateNoteCommand` | 204 |
| `DELETE /{noteId}` | Remove | `RemoveNoteCommand` | 204 |

---

## Rules

- Controllers contain NO logic — only `_mediator.Send()` + return ActionResult
- Every controller inherits `ControllerBase` with `[ApiController]` and `[Route("api/...")]`
- Create endpoints return `CreatedAtAction` (201) with Id
- Update/Delete return `NoContent()` (204)
- No try/catch in controllers — middleware handles exceptions globally

## Postman Testing

After each controller phase, create a corresponding **Postman folder** with all requests:

- `📁 Flats` — after Phase 2
- `📁 Tenants` — after Phase 3
- `📁 Chores` — after Phase 4
- `📁 Payments` — after Phase 5
- `📁 Notes` — after Phase 6

Collection uses `{{baseUrl}}` variable. Export to `docs/FlatFlow.postman_collection.json` after all phases are done.

## Execution Order

Phase 1 → 2 → 3 → 4 → 5 → 6. After each phase: build + verify + add Postman folder.
