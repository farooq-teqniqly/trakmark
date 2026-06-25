## 1. Domain: State

- [x] 1.1 Write failing tests in `Trakmark.Domain.Tests/ValueObjects/StateTests.cs` for the scenarios in `specs/manage-cities/spec.md` (equality by abbreviation, case-insensitive, inequality across distinct abbreviations) plus null-left operator coverage.
- [x] 1.2 Implement `Trakmark.Domain/ValueObjects/State.cs`: `sealed class State : IEquatable<State>` with 51 static readonly instances (50 states + D.C.), two-letter abbreviation identity, `==`/`!=` operators, `ArgumentNullException.ThrowIfNull` on any reference-type constructor parameters.
- [x] 1.3 Run tests to green.

## 2. Domain: CityId

- [x] 2.1 Write failing tests in `Trakmark.Domain.Tests/Ids/CityIdTests.cs` mirroring `SchoolIdTests.cs` (NewId uniqueness, Parse/TryParse round-trip, invalid format rejection). Cover `DomainId.IsValid` only via `TryParse`, not directly.
- [x] 2.2 Implement `Trakmark.Domain/Ids/CityId.cs` following the `SchoolId.cs` pattern (`readonly record struct`, `"CTY-"` prefix, backed by `DomainId`).
- [x] 2.3 Run tests to green.

## 3. Domain: City

- [x] 3.1 Write failing tests in `Trakmark.Domain.Tests/ValueObjects/CityTests.cs` for the scenarios in `specs/manage-cities/spec.md` (valid create, empty/whitespace name rejected, name over 100 chars rejected, null state rejected, equality by name+state, inequality on differing name or state) plus null-left operator coverage.
- [x] 3.2 Implement `Trakmark.Domain/ValueObjects/City.cs`: `sealed class City : IEquatable<City>` with `CityId`, `Name` (trimmed, max 100 chars), `State`; `Create(string name, State state)` static factory generating `CityId.NewId()`; `==`/`!=` operators; `ArgumentNullException.ThrowIfNull` on `name`/`state`.
- [x] 3.3 Run tests to green.

## 4. Persistence

- [x] 4.1 Add EF Core entity configuration for `City` (table, DB-generated `int IDENTITY` surrogate `Id` as the clustered primary key, `CityId` mapped as a unique non-clustered alternate key column, `Name` max length 100, `State` stored as the two-letter abbreviation, `CreatedAt` column, `CreatedByUserId` column storing the creating `RegisteredUserId` value).
- [x] 4.2 Add EF Core migration for the new `City` table.
- [x] 4.3 Write an integration test (Testcontainers) verifying a `City` round-trips through save/load with `CreatedAt` and `CreatedByUserId` set as persisted.

## 5. Application layer: batch save

- [x] 5.1 Write failing tests for a batch-save operation covering: all-valid batch persists all rows; one invalid row rejects the whole batch; in-batch duplicate rejects the whole batch; duplicate against existing persisted city rejects the whole batch; persisted rows carry the submitting Admin's `RegisteredUserId` as `CreatedByUserId`.
- [x] 5.2 Implement the batch-save operation: validate each submitted row via `City.Create`, check for in-batch duplicates via `City` equality, query existing cities for cross-batch duplicates, capture the current Admin's `RegisteredUserId` from the authenticated principal, persist all rows with `CreatedByUserId` set in a single transaction only if every check passes.
- [x] 5.3 Run tests to green.

## 6. UI: Add Cities page

- [ ] 6.1 Add "Add Cities" item to the Admin dropdown in the nav bar, restricted to the Admin role.
- [ ] 6.2 Build the Add Cities Blazor form: repeatable rows (1-100) with Name (text, max 100 chars) and State (dropdown), client-side validation as the user types, Save disabled until valid.
- [ ] 6.3 Wire Save to the batch-save operation; on success show a success toast and clear the form; on server validation/duplicate failure show the error and a save-failed toast.
- [ ] 6.4 Wire Cancel to navigate back to Home without modifying data.

## 7. Pre-merge

- [ ] 7.1 Register any new test/data-access projects in `Trakmark.slnx` if not already present.
- [ ] 7.2 Run the SonarQube clean-build warning check per `CLAUDE.md` and resolve or triage findings.
