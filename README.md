# Flat Flow

A full-stack web application for managing shared apartments — chores, payments, notes, and tenants. Built with **Clean Architecture** and **CQRS** pattern.

## Screenshots

<!-- Add your screenshots here -->

## Tech Stack

### Backend
- .NET 9 / ASP.NET Core
- Entity Framework Core + MSSQL
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- ASP.NET Identity + JWT Authentication
- Serilog

### Frontend
- Angular 21 (standalone components, signals)
- PrimeNG v19 (Aura dark theme)
- Reactive Forms
- Lazy-loaded feature modules

### Testing
- xUnit + FluentAssertions + Moq
- 471 tests (unit + integration)

## Architecture

The project follows **Clean Architecture** with strict layer separation:

```
src/
├── FlatFlow.Domain           # Entities, Value Objects, Domain Exceptions (zero dependencies)
├── FlatFlow.Application      # Use Cases (CQRS), DTOs, Repository Interfaces, Validators
├── FlatFlow.Infrastructure   # EF Core, Repositories, Identity, Migrations
├── FlatFlow.Api              # Controllers, Middleware, DI Configuration
tests/
├── FlatFlow.Domain.UnitTests
├── FlatFlow.Application.UnitTests
├── FlatFlow.Infrastructure.IntegrationTests
client/
├── Angular frontend
```

### Design Patterns
- **CQRS** — Commands (state changes) and Queries (data retrieval) separated via MediatR
- **Repository Pattern** — Abstracted data access with generic and specific repositories
- **Aggregate Root** — Flat manages Tenants/Chores/Notes/Payments; Payment manages Shares; Chore manages Assignments
- **Pipeline Behaviors** — Validation, Logging, and Exception handling in MediatR pipeline
- **JWT Authentication** — Flat-scoped authorization (Owner/Member policies)

## Features

- **Auth** — Register, login, JWT-based session, profile management
- **Flats** — Create flats, join via access code, manage settings
- **Chores** — CRUD with frequency (once/daily/weekly/monthly), assign to tenants, mark as complete/reopen
- **Payments** — CRUD with shares per tenant, mark as paid/partial
- **Notes** — CRUD with pagination
- **Tenants** — View members, promote/revoke ownership, remove (owner-only)
- **Settings** — Edit flat details, refresh access code, delete flat

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- SQL Server (LocalDB or full instance)

### Backend

```bash
# 1. Navigate to API project
cd src/FlatFlow.Api

# 2. Update connection string in appsettings.json if needed

# 3. Apply migrations
dotnet ef database update

# 4. Run
dotnet run
```

The API starts at `https://localhost:7248`.

### Frontend

```bash
# 1. Navigate to client
cd client

# 2. Install dependencies
npm install

# 3. Run
ng serve
```

The app starts at `http://localhost:4200`.

### Running Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test tests/FlatFlow.Domain.UnitTests
dotnet test tests/FlatFlow.Application.UnitTests
dotnet test tests/FlatFlow.Infrastructure.IntegrationTests
```

## API Endpoints

| Resource | Endpoints |
|----------|-----------|
| Auth | `POST /api/auth/register`, `POST /api/auth/login`, `GET/PUT /api/auth/me` |
| Flats | `GET /api/flats`, `GET /api/flats/my`, `POST /api/flats`, `GET/PUT/DELETE /api/flats/:id`, `POST /api/flats/join` |
| Chores | `GET/POST /api/flats/:id/chores`, `GET/PUT/DELETE .../chores/:id`, assignments CRUD |
| Payments | `GET/POST /api/flats/:id/payments`, `GET/PUT/DELETE .../payments/:id`, shares CRUD |
| Notes | `GET/POST /api/flats/:id/notes`, `GET/PUT/DELETE .../notes/:id` |
| Tenants | `GET /api/flats/:id/tenants`, promote, revoke, remove |
