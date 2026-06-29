## 1. Domain persistence infrastructure

- [x] 1.1 Add `RegisteredUserEntity` to `Trakmark/Data/Entities/` with `RegisteredUserId` (string, PK) and `AccountId` (string) columns
- [x] 1.2 Add `RegisteredUserConfiguration` to `Trakmark/Data/Configurations/` implementing `IEntityTypeConfiguration<RegisteredUserEntity>`; configure PK, column names, max lengths, and a unique index on `AccountId`
- [x] 1.3 Add `DbSet<RegisteredUserEntity> RegisteredUsers` to `ApplicationDbContext` and apply configuration in `OnModelCreating`
- [x] 1.4 Check for unrelated schema drift (`dotnet ef migrations add _check ...`), remove it, then scaffold migration `AddRegisteredUsers`

## 2. Audit infrastructure

- [x] 2.1 Add `IAuditableEntity` interface to `Trakmark/Data/` with `CreatedByUserId` (string) and `CreatedAt` (DateTimeOffset) properties
- [x] 2.2 Implement `ICurrentUserContext` interface and `CurrentUserContext` sealed class (scoped) in `Trakmark/Services/`; expose `RegisteredUserId? UserId { get; set; }`
- [x] 2.3 Add `AuditInterceptor : SaveChangesInterceptor` to `Trakmark/Data/`; in `SavingChangesAsync` stamp all `Added` `IAuditableEntity` entries with `ICurrentUserContext.UserId.Value` and `DateTimeOffset.UtcNow`; throw `InvalidOperationException` if `UserId` is null
- [x] 2.4 Mark `CityEntity` as implementing `IAuditableEntity` (properties already exist)
- [x] 2.5 Register `ICurrentUserContext` / `CurrentUserContext` as scoped and `AuditInterceptor` via `AddDbContext` `options.AddInterceptors(...)` in `ServiceCollectionExtensions.AddAppDatabase`

## 3. Registration hook — failing tests first

- [x] 3.1 Write failing integration tests in `Trakmark.IntegrationTests` for the `registered-user-identity-mapping` spec scenarios: (a) `RegisteredUser` row exists after new-user registration, (b) registration failure redirects to login; use `Assert.Fail("not implemented")` placeholder, confirm red, then replace
- [x] 3.2 Add `IRegisteredUserMappingService` interface and `RegisteredUserMappingService` to `Trakmark/Services/`; `CreateAsync(string identityUserId)` creates `RegisteredUser.Create(new UserAccountId(identityUserId))` and persists via `ApplicationDbContext`
- [x] 3.3 Register `IRegisteredUserMappingService` / `RegisteredUserMappingService` as scoped in `Program.cs`
- [x] 3.4 Inject `IRegisteredUserMappingService` into `ExternalLogin.razor`; in `CreateAndSignInUserAsync` call `CreateAsync(user.Id)` after `AddLoginAsync` succeeds; on failure redirect to `Account/Login` with error message and return
- [x] 3.5 Make integration tests green

## 4. Lookup service — failing tests first

- [x] 4.1 Write failing integration tests in `Trakmark.IntegrationTests` for `IRegisteredUserLookupService` spec scenarios: (a) returns `RegisteredUserId` for known user, (b) throws `InvalidOperationException` for unknown user; use `Assert.Fail("not implemented")` placeholder, confirm red, then replace
- [x] 4.2 Add `IRegisteredUserLookupService` interface and `RegisteredUserLookupService` to `Trakmark/Services/`; `GetByAccountIdAsync(string identityUserId)` queries `RegisteredUsers` by `AccountId`; throws `InvalidOperationException` if not found
- [x] 4.3 Register `IRegisteredUserLookupService` / `RegisteredUserLookupService` as scoped in `Program.cs`
- [x] 4.4 Make integration tests green

## 5. Wire AddCities and remove manual stamping — failing tests first

- [x] 5.1 Write failing bUnit tests in `Trakmark.Tests` covering `AddCities` with `ICurrentUserContext` set to a real `RegisteredUserId.NewId()` (not `Empty`); confirm existing tests that used `Empty` now fail; use `Assert.Fail("not implemented")` for new assertions
- [x] 5.2 Remove `RegisteredUserId createdByUserId` parameter from `ISaveCitiesBatchService.SaveAsync` and `SaveCitiesBatchService.SaveAsync` / `PersistAsync`; remove manual `CreatedByUserId` and `CreatedAt` stamping from `PersistAsync` (interceptor handles both)
- [x] 5.3 Inject `IRegisteredUserLookupService` and `ICurrentUserContext` into `AddCities.razor`; add `OnInitializedAsync` that resolves the `NameIdentifier` claim, calls `GetByAccountIdAsync`, and sets `ICurrentUserContext.UserId`; remove `GetCurrentUserIdAsync` method and the `userId` argument from the `BatchSaveService.SaveAsync` call in `SaveAsync`
- [x] 5.4 Update `AddCitiesTests` mocks/setup to remove `userId` argument from `SaveAsync` calls; set `ICurrentUserContext` via the `AuthenticationStateTask` / `TestAuthorizationContext` setup
- [x] 5.5 Update `SaveCitiesBatchTests` integration tests to remove `userId` argument; ensure `AuditInterceptor` stamps are verified through the persisted entity
- [x] 5.6 Make all updated tests green

## 6. AuditInterceptor and ICurrentUserContext tests

- [x] 6.1 Write unit tests in `Trakmark.Tests` (or a new `Trakmark.Tests` test class) for `AuditInterceptor` spec scenarios: (a) stamps `Added` `IAuditableEntity` with user ID and UTC time, (b) throws when `UserId` is null, (c) ignores non-`IAuditableEntity` entries; write as failing first, then implement to green
- [x] 6.2 Write unit tests for `CurrentUserContext`: (a) `UserId` is null on construction, (b) `UserId` returns set value; write as failing first, then implement to green

## 7. CLAUDE.md rules

- [x] 7.1 Add rule: "Every EF entity must have a dedicated `IEntityTypeConfiguration<T>` class in `Trakmark/Data/Configurations/`. Never configure entities inline in `OnModelCreating`."
- [x] 7.2 Add rule: "Use `IAuditableEntity` + `AuditInterceptor` for `CreatedByUserId` and `CreatedAt` — never stamp these fields manually in services or components."

## 8. Pre-merge

- [x] 8.1 Run SonarQube warning sweep (up to 3 rounds); record any unresolved items in `docs/sonarqube-warnings-triage.md` if rounds are exhausted
- [x] 8.2 Run `coverage-report` and confirm `Trakmark.Domain` line coverage remains 100% (this change does not touch Domain, but verify no regression)
- [x] 8.3 Confirm GitHub issue #22 is resolved: save a city and verify `CreatedByUserId` in the database is a real `USR-XXXXXXX` value, not `USR-AAAAAAA`
