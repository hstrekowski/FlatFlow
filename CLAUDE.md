# Project Guidelines (Clean Architecture + CQRS)

## Technical Stack

- **Backend:** .NET (latest LTS), ASP.NET Core, EF Core
- **Database:** MSSQL
- **Frontend:** Angular
- **Testing:** xUnit (Unit & Integration tests)
- **Key Libraries:** MediatR, FluentValidation, AutoMapper

## Architecture Overview

The project follows **Clean Architecture** principles to ensure separation of concerns and maintainability.

### Layer Responsibilities

1.  **Domain:** Contains Enterprise logic, Entities, Value Objects, and Domain Exceptions. **Zero dependencies** on other projects or frameworks.
2.  **Application:** Contains Business logic (Use Cases), DTOs, and **Repository Interfaces**. This layer uses **CQRS** with **MediatR**.
3.  **Infrastructure:** Contains the implementation of Repository interfaces, `DbContext`, Migrations, and external service integrations (e.g., File Storage, Identity).
4.  **Api:** The entry point. Contains Controllers, Middleware, Dependency Injection configuration, and `Program.cs`. Controllers should only trigger MediatR requests.

## Design Patterns & Rules

### CQRS & MediatR

- **Commands:** Used for operations that change state (Create, Update, Delete). Must return a result or ID.
- **Queries:** Used for data retrieval. Should not modify the database.
- **Handlers:** Every Command/Query must have a corresponding Handler in the **Application** layer.

### Repository Pattern

- **Interfaces:** Define all `IRepository` interfaces within the **Application** layer.
- **Implementation:** Implement these interfaces in the **Infrastructure** layer using EF Core.
- **Usage:** MediatR Handlers must inject the Repository interfaces to interact with the database. Do not use `DbContext` directly in Handlers.
- **Abstraction:** Avoid returning `IQueryable` from Repositories to keep EF Core logic encapsulated in Infrastructure. Return `Task<T>` or `Task<List<T>>`.

### Naming Conventions

- **Commands/Queries:** `[Action][Entity][Command/Query]` (e.g., `CreateUserCommand`, `GetProductByIdQuery`).
- **Handlers:** `[Command/QueryName]Handler`.
- **Repositories:** `I[Entity]Repository`.

## Project Structure

```text
src/
├── Project.Domain/
│   ├── Entities/
│   ├── Common/
├── Project.Application/
│   ├── Interfaces/ (IRepository resides here)
│   ├── Features/ (Folders per feature: Commands/Queries/Handlers)
│   ├── Common/ (Mappings, Behaviors)
├── Project.Infrastructure/
│   ├── Persistence/ (DbContext, Repository Implementations)
│   ├── Migrations/
├── Project.Api/
│   ├── Controllers/
│   ├── Middleware/
tests/
├── Project.Domain.UnitTests/
├── Project.Application.UnitTests/
├── Project.Infrastructure.IntegrationTests/
clinet/
├── Here goes Angular frotned
```
