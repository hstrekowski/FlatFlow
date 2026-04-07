# Flat Flow

A full-stack web application for managing shared apartments — chores, payments, notes, and tenants. Built with **Clean Architecture** and **CQRS** pattern. 

Full documentation continues below the gallery.

## Screenshots

Landing Page

<img width="1816" height="880" alt="login" src="https://github.com/user-attachments/assets/1c3ca5b8-755b-46b5-98fb-451a5ede477b" />

Sing-In Validation

<img width="392" height="424" alt="obraz" src="https://github.com/user-attachments/assets/541b27e0-7537-4df8-b364-ee538887591f" />

Sign-Up Validation

<img width="311" height="596" alt="obraz" src="https://github.com/user-attachments/assets/00444685-e55d-4a8b-b657-7635beb89391" />

Logged In View (No Flats)

<img width="1853" height="642" alt="obraz" src="https://github.com/user-attachments/assets/20521be5-4318-46f7-b076-07dbd5783c23" />

Profile Edit View (Validation works)

<img width="1850" height="550" alt="obraz" src="https://github.com/user-attachments/assets/3500c029-2c52-4efd-af0e-c43eb1af999b" />

Flat Creating Dialog

<img width="1231" height="808" alt="obraz" src="https://github.com/user-attachments/assets/f3756038-ae58-4278-988b-f80dc0e67634" />

Logged In View with Flats

<img width="1212" height="415" alt="obraz" src="https://github.com/user-attachments/assets/0329491b-a89f-432a-b7b6-e900f9023dda" />

Join By Access Code (Any Flat)

<img width="503" height="342" alt="obraz" src="https://github.com/user-attachments/assets/3310674e-c899-406d-9da6-6c7913cf2a8e" />

Join By Access Code Error (Any Flat)

<img width="438" height="334" alt="obraz" src="https://github.com/user-attachments/assets/fd2f998c-c19f-4071-bcbc-9aab5eeaf38f" />

User #2 View

<img width="1852" height="411" alt="obraz" src="https://github.com/user-attachments/assets/023eecb4-615c-40cd-8f62-3845a660aa05" />

Join Specific Flat Dialog 

<img width="437" height="281" alt="obraz" src="https://github.com/user-attachments/assets/5f044c2f-de99-446b-aa10-9ec28fbb1c91" />

Join Specific Flat Dialog Error

<img width="429" height="323" alt="obraz" src="https://github.com/user-attachments/assets/191b68d9-5b25-4c89-89f6-fb42336db13a" />

Flat Dashboard

<img width="1853" height="466" alt="obraz" src="https://github.com/user-attachments/assets/0af09310-8610-49ad-93bb-35ff836d2c8a" />

Empty Chore View

<img width="1151" height="364" alt="obraz" src="https://github.com/user-attachments/assets/b79b22eb-e5d5-4789-8dc6-5afba916b2e5" />

Add Chore Dialog

<img width="482" height="486" alt="obraz" src="https://github.com/user-attachments/assets/9ac26981-2ad4-45c8-a0d8-4a2489dbd44b" />

Chore View

<img width="1148" height="416" alt="obraz" src="https://github.com/user-attachments/assets/9a5b1e14-2446-4164-84cd-e879fc75f8e7" />

Add Chore Assignment Dialog

<img width="475" height="373" alt="obraz" src="https://github.com/user-attachments/assets/c05c1bce-7220-4a06-b89f-bec4a47b4c86" />

Chores View with Assignments

<img width="1161" height="476" alt="obraz" src="https://github.com/user-attachments/assets/d645693a-25e0-4367-803f-e0425bbe29f7" />

Payments View with Shares

<img width="1158" height="425" alt="obraz" src="https://github.com/user-attachments/assets/cc7c2592-495f-4255-b3ab-7999cb939d95" />

Notes View

<img width="1176" height="375" alt="obraz" src="https://github.com/user-attachments/assets/0878736f-ee1d-4974-869d-1ff2783fd0fe" />

Tenants View

<img width="1153" height="383" alt="obraz" src="https://github.com/user-attachments/assets/96056b23-b686-49db-a1f2-b7a1eb350427" />

Settings View

<img width="1036" height="915" alt="obraz" src="https://github.com/user-attachments/assets/849002e5-6d2e-4ff5-aed5-0daed9485b90" />


## Tech Stack

### Backend

- .NET 10 / ASP.NET Core
- Entity Framework Core + MSSQL
- ASP.NET Identity + JWT Authentication
- MediatR (CQRS) & Clean Architecture

### Frontend

- Angular 21, PrimeNG

### Testing

- xUnit + FluentAssertions + Moq
- 559 tests (unit + integration)

## Architecture

The project follows **Clean Architecture** with strict layer separation:

```
src/
├── FlatFlow.Domain           # Entities, Value Objects, Domain Exceptions
├── FlatFlow.Application      # CQRS, DTOs, Repository Interfaces, Validators
├── FlatFlow.Infrastructure   # EF Core, Repositories, Identity, Migrations
├── FlatFlow.Api              # Controllers, Middleware, DI Configuration
tests/
├── FlatFlow.Domain.UnitTests
├── FlatFlow.Application.UnitTests
├── FlatFlow.Infrastructure.IntegrationTests
client/
├── Angular frontend
```

## Features

- **Auth** — Register, login, JWT, profile management
- **Flats** — Create flats, join via access code, manage settings
- **Chores** — CRUD, assign to tenants, mark as complete/reopen
- **Payments** — CRUD with shares per tenant, mark as paid/partial
- **Notes** — CRUD with pagination
- **Tenants** — View members, promote/revoke ownership, remove (owner-only)
- **Settings** — Edit flat details, refresh access code, delete flat

## Getting Started

### Prerequisites

- .NET 10 SDK
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
