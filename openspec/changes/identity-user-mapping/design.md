## Context

ASP.NET Core Identity stores each user with a GUID string as the primary key (`AspNetUsers.Id`). Trakmark's domain uses `RegisteredUserId` (Crockford Base32, `USR-XXXXXXX`) as the application-level user identifier. These two systems have never been bridged: `AddCities` tries to parse the Identity GUID as a `RegisteredUserId`, always fails, and falls back to `RegisteredUserId.Empty`, persisting `USR-AAAAAAA` as the city creator.

The `RegisteredUser` domain aggregate already models the relationship correctly: `RegisteredUser.Create(UserAccountId)` generates a new `RegisteredUserId` and holds the `UserAccountId` (the Identity GUID). The fix is to persist this aggregate on registration and query it at runtime.

A secondary concern: `SaveCitiesBatchService` manually stamps `CreatedByUserId` and `CreatedAt`. As more tables are added, this pattern will be duplicated everywhere. An EF Core `SaveChangesInterceptor` centralizes the stamping.

A Blazor Server constraint shapes the interceptor design: `IHttpContextAccessor.HttpContext` is `null` during SignalR circuit execution (interactive server rendering). The interceptor cannot resolve the current user from the HTTP context. A scoped `ICurrentUserContext` — populated by the component on initialization — bridges the gap.

## Goals / Non-Goals

**Goals:**
- Bridge ASP.NET Identity GUIDs to `RegisteredUserId` by persisting `RegisteredUser` on first Google OAuth login.
- Provide `IRegisteredUserLookupService` to resolve `RegisteredUserId` from an Identity user ID.
- Introduce `IAuditableEntity` + `AuditInterceptor` to auto-stamp `CreatedByUserId` and `CreatedAt` on every `Added` entity, eliminating manual stamping in services.
- Introduce scoped `ICurrentUserContext` so Blazor Server components can supply the resolved user to the interceptor.
- Fix issue #22: cities are saved with the correct `RegisteredUserId`.

**Non-Goals:**
- Backfilling existing users — no users existed before this change.
- Updating existing cities — only new saves are affected.
- `UpdatedBy` / `UpdatedAt` audit fields — out of scope for this change.
- Role or permission changes.

## Decisions

### D1 — Persist `RegisteredUser` aggregate, not a flat mapping table

**Decision:** Persist `RegisteredUser` (with `Id` and `AccountId`) to a `RegisteredUsers` table.

**Rationale:** The domain already models the relationship. A separate `IdentityUserMappings` table would duplicate domain knowledge and create a second source of truth. Using the aggregate keeps domain and persistence aligned and positions the table for future columns (e.g., `Following`).

**Alternative considered:** A flat two-column infra table (`IdentityUserId`, `RegisteredUserId`). Rejected because it duplicates what `RegisteredUser` already expresses and bypasses the domain factory (`RegisteredUser.Create`).

---

### D2 — Hook in `ExternalLogin.razor`, `CreateAndSignInUserAsync`

**Decision:** After `AddLoginAsync` succeeds, call `RegisteredUser.Create(new UserAccountId(user.Id))` via `IRegisteredUserMappingService` and persist.

**Rationale:** `CreateAndSignInUserAsync` is the single path for new-user creation. The mapping must exist before the user's first interaction with any page that reads it. If mapping creation fails, redirect to login with an error — registration is atomic.

**Alternative considered:** `OnTicketReceived` event in Google OAuth middleware. Rejected: it fires on every login (not just new users), runs outside the Blazor DI scope, and requires a separate scope for EF work.

---

### D3 — Scoped `ICurrentUserContext` instead of `IHttpContextAccessor` in the interceptor

**Decision:** `ICurrentUserContext` is a scoped service with a `RegisteredUserId?` property. Authorized components set it in `OnInitializedAsync`. `AuditInterceptor` reads from it.

**Rationale:** `IHttpContextAccessor.HttpContext` is `null` in a Blazor Server SignalR circuit. The scoped `ICurrentUserContext` is set in the same scope as the `DbContext`, so the interceptor always has access without an HTTP dependency.

**Alternative considered:** Pass `RegisteredUserId` as a parameter through every service method. Rejected: it pollutes every service signature and must be repeated for every new table.

---

### D4 — `AuditInterceptor` throws if `ICurrentUserContext.UserId` is null

**Decision:** If `AuditInterceptor` encounters an `IAuditableEntity` in `Added` state but `ICurrentUserContext.UserId` is `null`, it throws `InvalidOperationException`.

**Rationale:** A null user ID means either the component forgot to set the context or the user has no mapping (registration bug). Silently stamping `Empty` is what caused issue #22. Failing loudly surfaces the problem immediately.

---

### D5 — Lookup failure throws, not returns a typed error

**Decision:** `RegisteredUserLookupService.GetByAccountIdAsync` throws `InvalidOperationException` when no mapping is found.

**Rationale:** A missing mapping means registration failed — an exceptional, unexpected state. Returning a typed error would force every call site to handle a case that should never occur in correct operation. Throwing surfaces the bug loudly rather than allowing silent fallback.

---

### D7 — Surrogate `int IDENTITY` PK; `RegisteredUserId` as alternate key

**Decision:** `RegisteredUsers` uses a DB-generated `int Id` as the clustered primary key. `RegisteredUserId` is mapped as a unique non-clustered alternate key via `HasAlternateKey`.

**Rationale:** Domain-assigned IDs are random strings (Crockford Base32). Using them as the clustered PK fragments the index on every insert and degrades range-scan performance. A sequential integer surrogate eliminates fragmentation. This is the same pattern used by `CityEntity` / `CityConfiguration`. The domain ID retains its uniqueness guarantee via the alternate key constraint.

**Alternative considered:** Keep `RegisteredUserId` as the PK with `ValueGeneratedNever()`. Rejected: random string clustering causes index fragmentation at scale.

---

### D6 — `RegisteredUserEntity` in `Trakmark/Data/Entities/`, config in `Trakmark/Data/Configurations/`

**Decision:** Follow the existing pattern: entity class in `Data/Entities/`, EF configuration in `Data/Configurations/` as a dedicated `IEntityTypeConfiguration<T>` class.

**Rationale:** Keeps all EF entity configurations consistent and discoverable. Avoids inline `OnModelCreating` configuration.

## Risks / Trade-offs

- **Component forgets to set `ICurrentUserContext`** → `AuditInterceptor` throws at save time. Mitigation: this is a loud failure caught immediately in dev; consider a base class `AuthorizedComponentBase` in a future change to enforce the pattern.
- **`RegisteredUser.Create` called inside the hook fails** → Registration is rolled back; user sees an error message and is redirected to login. No partial state is persisted.
- **`RegisteredUserLookupService` throws on missing mapping** → An unhandled exception in `AddCities.OnInitializedAsync` surfaces as an error page. Acceptable: this means registration is broken, which requires developer attention, not user-facing error handling.
- **EF interceptor ordering** → If other interceptors are registered in the future, stamping order may matter. Currently no other interceptors exist.

## Migration Plan

1. Add EF migration for the `RegisteredUsers` table.
2. Deploy — new user registrations immediately create mappings; existing cities retain `USR-AAAAAAA` (accepted, no backfill).
3. No rollback complexity: dropping the `RegisteredUsers` table and reverting the hook restores prior behavior.
