## Context

Trakmark is a .NET 10 Blazor Web App with ASP.NET Core Identity already in place (`ApplicationUser`, `ApplicationDbContext`). This change introduces a persistence-ignorant domain model for tracking a student athlete's track-and-field and cross-country results. It is a deliberately narrow vertical slice — one registered user, one student, hand-entered meets — chosen to validate the model shape before importers and multi-user concerns arrive.

The model was explored in `docs/narratives/domain-model-design.md` (scaffolding, to be removed). This document records the decisions that survived that exploration.

## Goals / Non-Goals

**Goals:**

- A rich, persistence-ignorant domain model where illegal states are unrepresentable and operations are non-throwing where practical.
- Four aggregates with clear consistency boundaries: `RegisteredUser`, `Student`, `Meet`, `School`.
- Derived Personal Best (PB) and Season Best (SB) per discipline, correct for both time and distance disciplines.
- Discipline-specific setup context (e.g. hurdle height) handled without a nullable swarm.
- Domain decoupled from ASP.NET Identity except for a single `UserAccountId` value.

**Non-Goals:**

- Persistence. No EF Core configuration, mappings, or migrations in this change.
- CSV import, multi-user, and student deduplication.
- Performance conditions (wind, timing type) and legal-PB gating.
- Relay teammate linkage and per-leg splits.
- The student-as-user path. `UserAccountId` on `Student` is modeled but dormant.

## Decisions

### Aggregate boundaries

Four roots, each the smallest unit loaded and saved together under one invariant:

- **RegisteredUser** — the account holder. Holds `Following`, a mutable set of `StudentId` (cross-aggregate reference by id). Bridged to Identity only by a `UserAccountId` value.
- **Student** — owns `Career`. A career is intrinsic to the student and its "one school per year" invariant is a student-level rule, so it lives inside the aggregate.
- **Meet** — owns `Result`s. Results are scored together when a meet is recorded, so write consistency makes the meet the boundary. A single student's results across meets are a *query*, not a stored collection on the student.
- **School** — owns `Team`s as a catalog of the sports it fields.

Alternative considered: putting `Result` on the `Student` aggregate (the narrative reads "their result is recorded"). Rejected — results are written per meet and the per-athlete season view is a read projection, so storing them on the student would bloat that aggregate without serving any write path.

### Season is the Enrollment; results map to it by date

`Career` is an ordered set of `Enrollment` value objects keyed by `SchoolYear`, with the invariant that no two enrollments share a year (no mid-year transfers). The current enrollment is the current season; past enrollments are past seasons — no separate `Season` type. A result maps to a season by its meet's date resolving to a `SchoolYear`.

### Result status governs mark and place presence

`Result` carries a `Status` ∈ { Finished, DidNotFinish, Disqualified, DidNotStart, NoMark }. Status drives optionality rather than free-floating nullables:

- `Finished` ⇒ `Mark` present (per the discipline's mark kind) **and** `Place` present.
- not `Finished` ⇒ `Mark` absent **and** `Place` absent.

This makes "a DNF with a time" or "a finish with no place" unrepresentable. `Order` records the entry sequence of a student's results within a meet (no heats concept; entry order is display order).

### Placement is not a Performance variant (Option A)

`Performance` is a closed hierarchy of **measured** marks — `TimeMark`, `DistanceMark` — only. Finish rank is a separate, uniform `Placement` carried as the optional `Place` on every result. A timed event can hold both a time and a place; a place-only discipline holds a place with no mark. This avoids placement playing a double role (a mark kind *and* a rank).

Alternative considered: keep `Placement` inside the `Performance` hierarchy for place-only disciplines. Rejected — it created ambiguous results carrying both a placement-mark and a place field.

### Discipline is a structured controlled vocabulary, and setup parameters are part of its identity

`Discipline` is a controlled vocabulary (not free text) so PB/SB grouping is reliable. Each discipline declares a `MarkKind` (time, distance, or place-only) that both constrains the valid `Performance` at result construction and implies the comparison direction (time → min wins, distance → max wins).

Discipline-specific **setup parameters** — hurdle height, implement weight — are structured typed fields that are part of the discipline's *identity* in the catalog. "110m hurdles @ 39"" and "100m hurdles @ 33"" are distinct disciplines. This:

- segments PB correctly — a 39" and a 33" hurdle time are never compared as the same best;
- avoids a nullable swarm — no `HurdleHeight?` / `ImplementWeight?` columns on `Result`;
- ages correctly — as an athlete moves up hurdle heights across seasons, their hurdle PB naturally splits.

This deliberately distinguishes **setup parameters** (define the contest, segment PB) from **performance conditions** (wind, timing type — qualify one mark, gate legal-PB), the latter deferred to a future `Result.Conditions` value object.

### Competitive tier is context, not part of discipline identity

A `Result` carries a `Tier` ∈ { Varsity, JV, Open } (a closed, extensible set) describing the competitive division a mark was set in. Tier is always present; when none is supplied it defaults to `Open`, so there is no absent/unknown tier to reason about downstream. Tier is deliberately distinct from `CompetitionLevel` (HS/MS/Elementary, which is a property of the school and the meet): a single HS meet can run Varsity and JV sections of the same event.

Crucially, tier is **tier-agnostic for bests** — PB and SB ignore it. An athlete's 100m personal best is their fastest legal time whether it was run in a JV or a Varsity race. This makes tier the opposite of a setup parameter such as hurdle height: hurdle height is part of the discipline's *identity* and segments PB; tier is contextual metadata that does *not* segment PB.

### PB and SB are derived read projections, never stored

- `SeasonBest(student, discipline, season)` = best `Finished` mark where the event's discipline matches, the season matches the enrollment's `SchoolYear`, the event is individual (not a relay), and a mark is present.
- `PersonalBest(student, discipline)` = the best across all seasons (the best over the season bests). Lifetime.

Direction comes from the discipline's `MarkKind`. Only `Finished` results count. Bests are defined only for measured disciplines; place-only disciplines have no measurable best. Relays are excluded (a team total time is not a personal best). Deriving rather than storing keeps these always correct with nothing to invalidate when a result is added or edited.

### Relays without a multi-student entity

A relay is an event whose discipline is a relay. Each participating student gets a normal per-student `Result` carrying the shared team total time (`TimeMark`). No multi-student entity, no teammate linkage, no leg splits. Relays are excluded from PB/SB. Revisit only if those become product goals.

### Cross-aggregate invariants enforced by a domain service

Two rules span aggregates and so cannot live transactionally inside one root:

- A meet's `CompetitionLevel` must match the student's enrollment level **for that meet's season** (the enrollment whose `SchoolYear` equals the meet's season, so historical meets validate against the then-current enrollment, not today's).
- Visibility: a user sees students = their `Following` set ∪ the student whose `UserAccountId` equals their account.

These are enforced at record/query time by a domain service. The sport-match rule (a result's event sport equals the meet's sport) is by contrast a *same-aggregate* invariant and is enforced inside `Meet` when recording a result.

### Strongly-typed IDs

`RegisteredUserId`, `StudentId`, `MeetId`, `SchoolId`, `TeamId`, `UserAccountId` — each a `readonly record struct` over a single value. They make wrong-id assignment a compile error and document which aggregate a reference points at. `UserAccountId` is the only bridge between the domain and the auth model.

**Identifier format.** The five domain-generated IDs carry a human-readable, per-type prefix followed by a hyphen and six Crockford base32 characters (uppercase `A`–`Z` and `2`–`9`, excluding the ambiguous `0`, `O`, `1`, `I`, `L`):

| ID | Prefix | Example |
|----|--------|---------|
| `StudentId` | `STU-` | `STU-7F3K9M` |
| `MeetId` | `MEET-` | `MEET-Q2X8RB` |
| `SchoolId` | `SCH-` | `SCH-4HJ7TN` |
| `TeamId` | `TEAM-` | `TEAM-9PXR2K` |
| `RegisteredUserId` | `USR-` | `USR-3MK8QF` |

The format is enforced in each ID's factory and parser (round-trippable `ToString` / `Parse`/`TryParse`), so an ill-formed value cannot be constructed. `NewId()` draws six random Crockford base32 characters and is treated as **effectively unique with no collision check** — acceptable at pilot volume; revisit with a uniqueness check if volume grows. `UserAccountId` is **exempt**: it wraps the external ASP.NET Identity `ApplicationUser` key, which the domain does not generate or reformat.

## Risks / Trade-offs

- **Relay time duplicated across legs' results** → Accepted; teammate linkage is not a product goal. A future multi-`StudentId` relay entry can become the single source of truth if needed.
- **Discipline catalog grows with every setup-parameter combination** (each hurdle height is a distinct discipline) → Accepted; the catalog is small and finite per sport/level, and the structured fields keep entries queryable rather than stringly-typed.
- **PB/SB derived on every read** → Acceptable at pilot scale (one student). If read cost grows with data, introduce a cached read model later without changing the domain.
- **Cross-aggregate level check needs the student's career at record time** → A domain service resolves the season's enrollment; if a career is incomplete, recording is blocked until the relevant enrollment exists. In the single-student pilot this is trivially satisfied.
- **Status/optionality invariant is easy to violate in persistence later** → Keep the invariant in the domain constructors/factories so any future EF Core mapping cannot bypass it.

## Open Questions

- Re-following after an unfollow is assumed unrestricted (idempotent add). Confirm when multi-follow UX is designed.
- Exact `MarkKind` set and the catalog of disciplines per sport/level will be enumerated during spec/implementation; the model only requires the three-way time/distance/place-only distinction.
