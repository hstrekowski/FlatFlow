# Project Guidelines (Clean Architecture + CQRS)

## Technical Stack

- **Backend:** .NET (latest LTS), ASP.NET Core, EF Core
- **Database:** MSSQL
- **Frontend:** Angular
- **Testing:** xUnit, FluentAssertions, Moq (Unit & Integration tests)
- **Key Libraries:** MediatR, FluentValidation, AutoMapper, Serilog

## Architecture Overview

The project follows **Clean Architecture** principles to ensure separation of concerns and maintainability.

### Layer Responsibilities

1.  **Domain:** Contains Enterprise logic, Entities, Value Objects, and Domain Exceptions. **Zero dependencies** on other projects or frameworks.
2.  **Application:** Contains Business logic (Use Cases), DTOs, and **Repository Interfaces** (in Contracts/Persistence). This layer uses **CQRS** with **MediatR**. Includes pipeline behaviors (Validation, Logging, UnhandledException).
3.  **Infrastructure:** Contains the implementation of Repository interfaces, `DbContext`, Migrations, and external service integrations (e.g., File Storage, Identity).
4.  **Api:** The entry point. Contains Controllers, Middleware, Dependency Injection configuration, and `Program.cs`. Controllers should only trigger MediatR requests.

## Design Patterns & Rules

### CQRS & MediatR

- **Commands:** Used for operations that change state (Create, Update, Delete). Return Guid for creates, Unit for updates/deletes.
- **Queries:** Used for data retrieval. Should not modify the database.
- **Handlers:** Every Command/Query must have a corresponding Handler in the **Application** layer.
- **Validators:** Every Command has a FluentValidation validator. ValidationBehavior runs them automatically in the MediatR pipeline.

### Repository Pattern

- **Interfaces:** Define `IGenericRepository<T>` and specific `I[Entity]Repository` interfaces in **Application/Contracts/Persistence**.
- **Implementation:** Implement these interfaces in the **Infrastructure** layer using EF Core.
- **Usage:** MediatR Handlers inject Repository interfaces. Do not use `DbContext` directly in Handlers.
- **Abstraction:** Avoid returning `IQueryable` from Repositories. Return `Task<T>`, `Task<List<T>>`, or `Task<PaginatedResult<T>>`.
- **No repositories for child entities** managed by aggregate roots (PaymentShare, ChoreAssignment). Access them through their parent's repository.

### Aggregate Root Pattern

- **Flat** manages: Tenants, Chores, Notes, Payments (via Add/Remove methods)
- **Payment** manages: PaymentShares (via AddShare/RemoveShare)
- **Chore** manages: ChoreAssignments (via AddAssignment/RemoveAssignment)
- Collections are exposed as `IReadOnlyList<T>` — modification only through aggregate root methods.
- Child entity commands must include parent ID (e.g., CompleteChoreAssignmentCommand has ChoreId + AssignmentId).

### Domain Exceptions

- `DomainException` — state violations (e.g., "Tenant is already an owner") → API returns 422
- `DomainValidationException` — input validation failures (e.g., "First name cannot be empty"), includes PropertyName → API returns 400

### Application Exceptions

- `NotFoundException` — entity not found in database → API returns 404
- `ForbiddenException` — insufficient permissions → API returns 403
- `ValidationException` (FluentValidation) — invalid command input → API returns 400

### Naming Conventions

- **Commands/Queries:** `[Action][Entity][Command/Query]` (e.g., `CreateFlatCommand`, `GetFlatByIdQuery`).
- **Handlers:** `[Command/QueryName]Handler`.
- **Repositories:** `I[Entity]Repository`.
- **DTOs:** `[Entity]Dto`, `[Entity]DetailDto` (detail includes child collections).
- **Feature folders:** Singular names (Flat, Tenant, Chore, Payment, Note).
- **DTOs live inside Queries/** — they are for data retrieval only, not commands.

### Mapping

- AutoMapper profiles in `Common/Mappings/`, one file per feature (e.g., `FlatMappingProfile.cs`).
- Mapping is only Entity → DTO (read direction). Commands use their own properties, not DTOs.

### Testing

- Every piece of code must have corresponding unit tests.
- Test project mirrors source structure (Entities/, Common/, ValueObjects/).
- Use `// Arrange`, `// Act`, `// Assert` comments in every test.
- Use Moq for mocking repositories and ILogger in Application tests.

## Project Structure

```text
src/
├── FlatFlow.Domain/
│   ├── Common/              (BaseEntity)
│   ├── Entities/            (Flat, Tenant, Payment, PaymentShare, Chore, ChoreAssignment, Note)
│   ├── Enums/               (ChoreFrequency, PaymentShareStatus)
│   ├── Exceptions/          (DomainException, DomainValidationException)
│   └── ValueObjects/        (Address)
├── FlatFlow.Application/
│   ├── Common/
│   │   ├── Behaviors/       (ValidationBehavior, LoggingBehavior, UnhandledExceptionBehavior)
│   │   ├── Exceptions/      (NotFoundException, ForbiddenException)
│   │   ├── Mappings/        (one Profile per feature)
│   │   └── Models/          (PaginatedResult)
│   ├── Contracts/
│   │   └── Persistence/     (IGenericRepository, IFlatRepository, ITenantRepository, etc.)
│   ├── Features/
│   │   ├── Flat/            (Commands/, Queries/)
│   │   ├── Tenant/
│   │   ├── Chore/
│   │   ├── Payment/
│   │   └── Note/
│   └── ApplicationServiceRegistration.cs
├── FlatFlow.Infrastructure/
│   ├── Persistence/         (DbContext, Repository Implementations)
│   └── Migrations/
├── FlatFlow.Api/
│   ├── Controllers/
│   └── Middleware/
tests/
├── FlatFlow.Domain.UnitTests/
│   ├── Common/              (BaseEntityTests)
│   ├── Entities/            (FlatTests, TenantTests, ChoreTests, etc.)
│   └── ValueObjects/        (AddressTests)
├── FlatFlow.Application.UnitTests/
├── FlatFlow.Infrastructure.IntegrationTests/
client/
├── Angular frontend
```

## Current Status

- **Domain layer: COMPLETE** — 272 unit tests, all passing
- **Application layer: PLANNED** — see PLAN_APPLICATION.md
- **Infrastructure layer: NOT STARTED**
- **API layer: NOT STARTED**
