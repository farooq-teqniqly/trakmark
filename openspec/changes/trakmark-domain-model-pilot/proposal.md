## Why

Trakmark needs a rich domain model before features accrete around an anemic one. A track-and-field parent should be able to record their child's meet results and watch marks improve over time — the app's whole reason to exist. This change captures the first buildable slice: one registered user, one student, hand-entered meets, with personal and season bests derived from the data.

## What Changes

- Introduce four domain aggregates: `RegisteredUser`, `Student` (with `Career`), `Meet` (with `Result`), and `School` (with `Team`).
- A registered user adds a `Student`, which both creates the canonical record and follows it in one act.
- Record a student's `Career` as `Enrollment`s — one school per `SchoolYear`, no mid-year transfers. The current enrollment is the current season; past enrollments are past seasons.
- Record `Meet`s (single sport, single competition level) and ordered `Result`s for a student, each with a `Status` (Finished, DidNotFinish, Disqualified, DidNotStart, NoMark) and a competitive `Tier` (Varsity, JV, Open), defaulting to `Open` when none is given.
- Model `Discipline` as a structured controlled vocabulary whose identity includes setup parameters (e.g. hurdle height), so personal bests segment correctly and never collapse into a nullable-swarm of per-discipline columns.
- Derive **Personal Best** and **Season Best** per discipline (never stored), excluding non-finishes and relays.
- Enforce two cross-aggregate invariants at record time via a domain service: a meet's level must match the student's enrollment level for that season, and a result's event sport must match the meet's sport.
- Keep the domain persistence- and auth-ignorant; the only link to ASP.NET Identity is a `UserAccountId` value.
- Give domain-generated IDs a human-readable, per-type prefixed format (`STU-`, `MEET-`, `SCH-`, `TEAM-`, `USR-`) followed by six Crockford base32 characters; `UserAccountId` keeps the external ASP.NET Identity key unchanged.

**Deferred (explicitly out of scope):** CSV import, multi-user and student deduplication, performance conditions (wind, timing type) and legal-PB gating, relay teammate linkage and leg splits, and the student-as-user path (`UserAccountId` on `Student` is modeled but dormant).

## Capabilities

### New Capabilities

- `manage-students`: A registered user adds a student (create + follow), records the student's career as enrollments, and unfollows.
- `manage-schools`: Create a school with its competition level and the sports it fields.
- `record-meet-results`: Create a meet (single sport, single level) and record ordered results for a student with a status; enforce the sport-match and level-match invariants; handle relay shared-time results.
- `view-athlete-season`: View a student's season (current and past, by enrollment) and list that season's events and results in entry order.
- `personal-and-season-best`: Compute derived personal best and season best per discipline with the correct direction, excluding non-finishes and relays.
- `domain-identity-format`: Domain-generated IDs use a human-readable, per-type prefixed format `PREFIX-XXXXXX` (six Crockford base32 characters); `UserAccountId` is exempt.

### Modified Capabilities

<!-- None. This is a greenfield domain model; no existing capability requirements change. -->

## Impact

- New domain project (`Trakmark.Domain`), persistence-ignorant class library; no EF Core mapping or migrations in this change. Unit tests live in `Trakmark.Domain.Tests`.
- Strongly-typed IDs: `RegisteredUserId`, `StudentId`, `MeetId`, `SchoolId`, `TeamId`, `UserAccountId`.
- Bridges to existing ASP.NET Identity (`ApplicationUser`) only through `UserAccountId`; no change to auth flow.
- Supersedes `docs/narratives/domain-model-design.md`, which was exploratory scaffolding and can be removed once this change is the source of truth.
- No database schema changes, no new dependencies in this slice.
