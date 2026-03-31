# Plan: Authentication & Authorization Layer

## Context

API layer is complete (40 endpoints, 5 controllers). All endpoints are currently unprotected. Need to add ASP.NET Identity + JWT authentication and role-based authorization per flat.

**No global admin role.** Authorization is flat-scoped: Owner vs Member.

---

## Decisions

- **Auth provider:** ASP.NET Identity + JWT
- **Token:** Access token only (60 min), no refresh token (portfolio scope)
- **Database:** Same MSSQL database, Identity tables alongside existing ones
- **Email verification:** Skipped for now
- **Roles:** No global roles. Authorization based on Tenant.IsOwner per flat
- **User ↔ Tenant:** Creating a Flat auto-creates owner Tenant. Joining via AccessCode auto-creates member Tenant. Profile data (firstName, lastName, email) pulled from Identity user.

---

## Authorization Rules

**Unauthenticated:**
- Register, Login only

**Authenticated (no flat context):**
- Create Flat (auto-becomes owner)
- Join Flat via AccessCode (auto-becomes member)
- View own profile, update own profile
- Browse flats list (no details)

**Owner (Tenant.IsOwner = true in given flat):**
- Everything in their flat — CRUD flat, manage tenants, all chores/payments/notes operations
- Cannot manage flats where they are not owner

**Member (Tenant.IsOwner = false in given flat):**
- View flat details, tenants list
- Add chores, payments, notes
- Delete/edit only own chores, payments, notes
- Complete/Reopen only assignments assigned to them
- Mark as paid only shares assigned to them
- Leave flat (self-remove)
- Cannot: delete/edit flat, add/remove/promote tenants

---

## Phase 1: Identity Setup (Infrastructure)

**Goal:** Add ASP.NET Identity to existing DbContext, create ApplicationUser entity, generate migration.

**Files:**
- `Infrastructure/Identity/ApplicationUser.cs` — extends IdentityUser, adds FirstName, LastName
- `Infrastructure/Persistence/FlatFlowDbContext.cs` — change base to IdentityDbContext<ApplicationUser>
- `Infrastructure/InfrastructureServiceRegistration.cs` — add Identity + JWT services
- Migration: `AddIdentity`

**ApplicationUser properties:**
- Inherits: Id (string), Email, UserName, PasswordHash (from IdentityUser)
- Added: FirstName (string), LastName (string)

---

## Phase 2: Auth Service (Infrastructure)

**Goal:** Create auth service that handles register, login, token generation.

**Files:**
- `Application/Contracts/Identity/IAuthService.cs` — interface
- `Application/Models/Identity/AuthRequest.cs` — login model
- `Application/Models/Identity/AuthResponse.cs` — token response
- `Application/Models/Identity/RegistrationRequest.cs` — register model
- `Infrastructure/Identity/AuthService.cs` — implementation

**AuthService methods:**
- `RegisterAsync(RegistrationRequest)` → AuthResponse
- `LoginAsync(AuthRequest)` → AuthResponse
- `GenerateJwtToken(ApplicationUser)` → string (private)

**JWT config in appsettings:**
```json
"JwtSettings": {
  "Key": "super-secret-key-min-32-chars",
  "Issuer": "FlatFlow",
  "Audience": "FlatFlowUsers",
  "ExpirationInMinutes": 60
}
```

---

## Phase 3: Auth Controller (API)

**Route:** `api/auth`

| Endpoint | Method | Description |
|---|---|---|
| `POST /register` | Register | Create user, return JWT |
| `POST /login` | Login | Validate credentials, return JWT |
| `GET /me` | GetProfile | Get current user profile |
| `PUT /me` | UpdateProfile | Update firstName, lastName, email |

---

## Phase 4: Protect All Endpoints

**Goal:** Add `[Authorize]` to all controllers. Configure JWT authentication middleware.

**Files:**
- `Program.cs` — add Authentication + Authorization middleware
- All controllers — add `[Authorize]` attribute
- `AuthController` — `[AllowAnonymous]` on register/login

After this phase: no endpoint works without a valid JWT (except register/login).

---

## Phase 5: Flat-Scoped Authorization

**Goal:** Implement authorization that checks Tenant membership and ownership per flat.

**Approach:** Custom authorization handler that reads `flatId` from route, checks if current user is a Tenant in that flat, and whether they are Owner.

**Files:**
- `Api/Authorization/FlatMemberRequirement.cs` — policy: user must be tenant in flat
- `Api/Authorization/FlatOwnerRequirement.cs` — policy: user must be owner in flat
- `Api/Authorization/FlatAuthorizationHandler.cs` — reads flatId from route, checks DB
- `Program.cs` — register policies

**Usage on controllers:**
```csharp
[Authorize(Policy = "FlatMember")]  // any tenant in this flat
[Authorize(Policy = "FlatOwner")]   // owner only
```

---

## Phase 6: Resource-Level Authorization

**Goal:** Enforce "own resource" rules — member can only delete/edit their own chores, payments, notes. Complete/reopen only own assignments. Mark paid only own shares.

**Approach:** In handlers, compare current userId with resource's creator/assignee. If not match and not owner → throw ForbiddenException.

**Files:**
- `Application/Contracts/Identity/ICurrentUserService.cs` — interface to get current user
- `Api/Services/CurrentUserService.cs` — reads userId from HttpContext.User claims
- Modify relevant handlers to check ownership

**Rules encoded in handlers:**
- Delete/Update chore → creator or owner
- Delete/Update payment → creator or owner
- Delete/Update note → author or owner
- Complete/Reopen assignment → assigned tenant or owner
- Mark share paid/partial → tenant on share or owner

---

## Phase 7: Refactor CreateFlat & JoinFlat

**Goal:** Auto-create Tenant when user creates or joins flat. Remove manual userId/profile data from commands.

**Changes:**
- `CreateFlatCommand` — remove need for manual tenant creation, handler auto-creates owner Tenant using ICurrentUserService
- `JoinFlatCommand` — simplify to just AccessCode, handler pulls user data from ICurrentUserService
- `AddTenantCommand` — owner-only (manual add), still takes profile data

---

## Phase 8: Postman Collection + Final Testing

**Goal:** Generate Postman collection with all endpoints including auth headers.

**Files:**
- `docs/FlatFlow.postman_collection.json`
- Includes: register → login → use token in all requests
- Folder structure: Auth, Flats, Tenants, Chores, Payments, Notes

---

## Rules

- Identity is an Infrastructure concern — Application only sees interfaces (IAuthService, ICurrentUserService)
- Controllers never check authorization logic — it's in policies or handlers
- JWT secret NEVER in tracked files — only in appsettings.Development.json (gitignored)
- All existing unit tests must still pass — authorization is an API/Infrastructure concern

## Execution Order

Phase 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8. After each phase: build + verify.
