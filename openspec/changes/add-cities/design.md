## Context

`Trakmark.Domain` already has a precedent for closed-set value objects (`Sport`, `CompetitionLevel`) and for domain-generated, string-prefixed strongly-typed IDs (`SchoolId`, via the shared `DomainId` helper). This change introduces the first two reference-data types — `State` and `City` — that future capabilities (`manage-school-districts`, `manage-schools`) will depend on. Getting their shape right matters because districts and schools will embed `City`/`State` directly.

## Goals / Non-Goals

**Goals:**
- Model `State` as a closed, in-code set of value objects (50 states + DC), consistent with the existing `Sport`/`CompetitionLevel` pattern — no DB table, no migration needed to add a state.
- Model `City` as a domain type enforcing its own invariants (name length, required state) with `Name`+`State` equality.
- Define a domain-level batch validation outcome that the application layer can use to implement the all-or-nothing save described in the use case (the duplicate check itself depends on persisted data, so it lives outside the domain).
- Establish a `CityId` strongly-typed ID following the `SchoolId` pattern.

**Non-Goals:**
- Building the Blazor "Add Cities" page, EF Core entity/configuration, or migration — those are implementation tasks tracked in `tasks.md`, not domain design decisions.
- Modeling `SchoolDistrict` or extending `School` with location fields (out of scope per proposal).
- Authorization/role-check implementation (Admin-only) — handled by existing ASP.NET Core Identity infrastructure, not new domain logic.

## Decisions

### `State` is a closed-set value object, not a DB-seeded table
Mirrors `Sport`: `sealed class State : IEquatable<State>` with 51 static readonly instances (50 states + DC), keyed by two-letter abbreviation (ordinal, case-insensitive compare on the abbreviation). Rejected DB-seeded-table alternative: states change essentially never, and a closed set lets `City` embed `State` directly without an extra FK/lookup join, consistent with how `School` embeds `Sport` rather than referencing a `SportId`.

### `City` is a `sealed class`, not a `record`
`City` enforces invariants (non-empty name, max 100 characters, non-null state) in its constructor and defines custom equality (`Name` + `State`, case-insensitive name), so per project convention it must be `sealed class` with `IEquatable<City>` and `==`/`!=` operators — not a `record`.

### `CityId` follows the `SchoolId` pattern
`readonly record struct CityId` backed by `DomainId`, prefix `"CTY-"`, generated via `CityId.NewId()` in `City.Create(...)`. This keeps ID generation in the domain (consistent with `School.Create`) as the business identifier used by other aggregates and the application layer.

### `City` table uses a DB-generated `int IDENTITY` clustered key, not `CityId`
`CityId` is a `DomainId`-backed string (`"CTY-"` + GUID-derived value), which is effectively random. Using it as the clustered primary key would cause page splits and index fragmentation on every insert — poor fit for a clustered index, which performs best on a narrow, sequential key. Instead: the `City` table gets a DB-generated `int IDENTITY(1,1)` surrogate column (`Id`) as the clustered primary key, and `CityId` is mapped as a separate `nvarchar` column with a unique non-clustered index (alternate key). The domain and application layer continue to use `CityId` exclusively — the surrogate `Id` is a persistence-only implementation detail, never exposed outside the EF Core configuration/entity.

### `CreatedByUserId` is persistence metadata, not a `City` constructor parameter
Like `CreatedAt`, the creating user is recorded by the persistence layer at save time, not passed into `City.Create(...)`. `City`'s domain invariants (name, state) don't depend on who created it, so adding a user parameter to the constructor would conflate domain validity with audit metadata. The application layer's batch-save operation captures the current Admin's `RegisteredUserId` (from the authenticated principal) and passes it to the persistence layer alongside each `City`, where the EF Core entity configuration maps it to a `CreatedByUserId` column (`nvarchar`, FK-shaped reference to `RegisteredUserId.Value`, not a DB FK constraint since `RegisteredUser` persistence is out of scope for this change).

### Duplicate detection is an application-layer concern
`City.Equals` provides the means to compare two `City` instances, but the domain has no repository access. The application/persistence layer is responsible for: (1) validating each row in the submitted batch using `City`'s constructor (catching invariant violations), (2) checking the batch for in-batch duplicates via `City` equality, and (3) querying existing persisted cities for cross-batch duplicates. All three failure modes reject the entire batch (all-or-nothing).

## Risks / Trade-offs

- **[Risk]** Hardcoding 51 `State` instances means adding a territory (e.g., Puerto Rico) later requires a code change and deployment. → **Mitigation**: Acceptable trade-off per explicit decision; revisit as a DB-backed table only if the set is shown to need runtime extension.
- **[Risk]** All-or-nothing batch save on a 100-row form could frustrate users who get one row wrong. → **Mitigation**: Client-side validation (already specified in the use case) catches most errors before submission; server-side duplicate checks are the main residual failure mode.

## Open Questions

- None outstanding — implementation-level questions (EF configuration, page layout) are deferred to `tasks.md`.
