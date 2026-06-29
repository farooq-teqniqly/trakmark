## Why

Every city saved by an Admin is recorded with `USR-AAAAAAA` (the sentinel empty `RegisteredUserId`) as the creator because no mapping exists between ASP.NET Identity's GUID claim and Trakmark's `RegisteredUserId`. This makes the `CreatedByUserId` column meaningless and will affect every future table that records the creating user.

## What Changes

- **NEW** — Persist the `RegisteredUser` aggregate to SQL Server on first Google OAuth login; the aggregate already models the `UserAccountId → RegisteredUserId` relationship.
- **NEW** — `IRegisteredUserLookupService` resolves a `RegisteredUserId` from an ASP.NET Identity user ID.
- **NEW** — `IAuditableEntity` interface + `AuditInterceptor` (`SaveChangesInterceptor`) stamps `CreatedByUserId` and `CreatedAt` on every `Added` entity automatically — no manual stamping in services.
- **NEW** — Scoped `ICurrentUserContext` bridges the resolved `RegisteredUserId` to the EF interceptor inside a Blazor Server SignalR circuit (where `IHttpContextAccessor.HttpContext` is `null`).
- **MODIFIED** — `ExternalLogin.razor` `CreateAndSignInUserAsync`: after `AddLoginAsync` succeeds, create and persist a `RegisteredUser`.
- **MODIFIED** — `AddCities.razor` `OnInitializedAsync`: resolve current user via lookup, set `ICurrentUserContext`; `SaveAsync` drops the `userId` parameter.
- **MODIFIED** — `SaveCitiesBatchService.SaveAsync`: drops `RegisteredUserId` parameter; `PersistAsync` removes manual `CreatedByUserId`/`CreatedAt` stamping (interceptor handles it).
- **MODIFIED** — `CLAUDE.md`: codify (a) every EF entity requires an `IEntityTypeConfiguration<T>` in `Data/Configurations/`, and (b) use `IAuditableEntity` + `AuditInterceptor` for creation metadata — never stamp manually in services.

## Capabilities

### New Capabilities

- `registered-user-identity-mapping`: Persist `RegisteredUser` on first Google OAuth login and resolve `RegisteredUserId` from an ASP.NET Identity user ID at runtime.
- `entity-audit-interceptor`: Cross-cutting EF Core interceptor that auto-stamps `CreatedByUserId` and `CreatedAt` on any entity implementing `IAuditableEntity`, fed by a scoped `ICurrentUserContext`.

### Modified Capabilities

- `google-oauth-auto-registration`: On successful new-user registration, also create and persist a `RegisteredUser` aggregate to establish the identity mapping.
- `manage-cities`: The "creating user is recorded on persistence" requirement is now fulfilled via `AuditInterceptor` rather than an explicit service parameter; `SaveCitiesBatchService` no longer accepts or stamps a `RegisteredUserId`.

## Impact

- `Trakmark` (web project): `ExternalLogin.razor`, `AddCities.razor`, `SaveCitiesBatchService`, `ISaveCitiesBatchService`, `ApplicationDbContext`, `ServiceCollectionExtensions`, `Program.cs`
- New files: `RegisteredUserEntity`, `RegisteredUserConfiguration`, `IAuditableEntity`, `ICurrentUserContext`, `CurrentUserContext`, `AuditInterceptor`, `IRegisteredUserLookupService`, `RegisteredUserLookupService`, EF migration
- `Trakmark.Tests`: `AddCitiesTests` updated — `NameIdentifier` claim uses real `RegisteredUserId.NewId()` instead of `RegisteredUserId.Empty`
- `Trakmark.IntegrationTests`: `SaveCitiesBatchTests` updated; new `RegisteredUserLookupServiceTests`
- `CLAUDE.md`: two new rules
- Closes GitHub issue #22
