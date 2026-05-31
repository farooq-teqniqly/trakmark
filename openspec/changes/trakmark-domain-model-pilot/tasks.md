# Tasks

Work test-first (TDD). In each section, complete the `.T` task — write failing tests derived from the cited spec scenarios — **before** the implementation tasks, then implement until those tests are green. Tests assert observable behavior, not internals.

## 1. Strongly-typed IDs and shared primitives

- [ ] 1.0 Create `Trakmark.Domain` class library (net10.0) and `Trakmark.Domain.Tests` xUnit project; reference Domain from Tests; add both to `Trakmark.slnx`
- [ ] 1.T Write failing tests from `domain-identity-format` spec — per-type prefix and six-char Crockford body (`New id matches the type's format`, `Each aggregate type uses its own prefix`), rejection of wrong prefix / out-of-charset / wrong-length (`Ill-formed identifiers cannot be constructed`), text round-trip, and `UserAccountId` exemption; plus value-object invariant tests for `PersonName`/`SchoolName`/`MeetName`/`TeamName` (non-empty/trim) and the closed sets. Use `[Theory]` for the format/charset cases.
- [ ] 1.1 Add `RegisteredUserId`, `StudentId`, `MeetId`, `SchoolId`, `TeamId`, `UserAccountId` as `readonly record struct`s with `Empty` and `NewId()`
- [ ] 1.1a Add a Crockford base32 generator (uppercase, excluding `0/O/1/I/L`) and the per-type prefix format (`STU-`/`MEET-`/`SCH-`/`TEAM-`/`USR-` + six chars); implement `NewId()`, `ToString`, `Parse`/`TryParse` so ill-formed values cannot be constructed; keep `UserAccountId` exempt (wraps the raw Identity key)
- [ ] 1.2 Add `PersonName`, `SchoolName`, `MeetName`, `TeamName` value objects with non-empty/trim validation
- [ ] 1.3 Add `SchoolYear` (orderable academic year), `GradeLevel` (closed set), `CompetitionLevel` (HS/MS/Elementary closed set), and `Sport` (Track & Field / Cross-Country closed set)
- [ ] 1.4 Add `MeetDate` value object over `DateOnly`

## 2. Discipline catalog and performance marks

- [ ] 2.T Write failing tests for `MarkKind` comparison direction (time → min wins, distance → max wins), `Discipline` identity (distinct setup parameters yield distinct disciplines — e.g. differing hurdle height), `Performance` comparability per direction, and `Event` exposing relay-ness from its discipline. Use `[Theory]` for the direction/identity cases.
- [ ] 2.1 Define `MarkKind` (time, distance, place-only) and the comparison direction it implies
- [ ] 2.2 Model `Discipline` as a structured controlled vocabulary whose identity includes setup parameters (e.g. hurdle height, implement weight); ensure distinct parameters yield distinct disciplines
- [ ] 2.3 Define `Performance` as a closed hierarchy of measured marks: `TimeMark`, `DistanceMark`, each comparable per the discipline direction
- [ ] 2.4 Define `Placement` (positive-int finish rank) and `Tier` (Varsity/JV/Open, closed and extensible)
- [ ] 2.5 Define `Event` value object pairing a `Discipline` with its `Sport`; expose relay-ness from the discipline

## 3. School aggregate

- [ ] 3.T Write failing tests from `manage-schools` spec — `Create a school`, `School name must be valid`, `Add a team for a sport`, and `Roster membership is not stored on the team`.
- [ ] 3.1 Implement `School` root with `SchoolName` and one `CompetitionLevel`
- [ ] 3.2 Add `Team` entity bound to one `Sport` and a method to add a team for a sport not yet fielded
- [ ] 3.3 Verify no roster is stored on `Team` (membership is derived)

## 4. Student aggregate and career

- [ ] 4.T Write failing tests from `manage-students` spec (career) — `Add an enrollment for a new school year` (latest year is current), `Reject a duplicate school year` (career unchanged), `Grade level is recorded independently`, and the empty-career-until-enrollment case from `Adding a student creates and follows it`.
- [ ] 4.1 Implement `Enrollment` value object `(SchoolId, SchoolYear, GradeLevel)`
- [ ] 4.2 Implement `Career` value object enforcing one enrollment per `SchoolYear` and deriving `Current` as the latest year
- [ ] 4.3 Implement `Student` root with `PersonName`, optional `UserAccountId` (dormant), and `Career`; add enrollment operations that reject duplicate years
- [ ] 4.4 Expose season helpers (current season = current enrollment; past seasons = prior enrollments)

## 5. RegisteredUser aggregate

- [ ] 5.T Write failing tests from `manage-students` spec — `Adding a student creates and follows it`, `Person name must be valid`, `Unfollow removes the link only` (student still exists), `Following is idempotent`, and `A student has no account link by default`.
- [ ] 5.1 Implement `RegisteredUser` root with `UserAccountId` and a mutable `Following` set of `StudentId`
- [ ] 5.2 Implement idempotent `Follow` and `Unfollow` operations
- [ ] 5.3 Implement `AddStudent` that creates a `Student` and follows it in one operation

## 6. Meet aggregate and result recording

- [ ] 6.T Write failing tests from `record-meet-results` spec — `Create a meet`, `Tier defaults to Open`, `Record a result with a competitive tier`, `A finished result requires a mark and a place`, `A non-finished result carries neither mark nor place`, `Results preserve entry order`, `Reject a mismatched mark`, `A place-only discipline takes a place and no mark`, `Reject a cross-sport event`, and `Each relay leg's student gets the team time`. Drive the status × mark/place matrix with `[Theory]`.
- [ ] 6.1 Implement `Meet` root with `MeetName`, `MeetDate`, one `CompetitionLevel`, one `Sport`, and a results collection
- [ ] 6.2 Implement `Result` entity with `StudentId`, `Event`, optional `Mark`, optional `Place`, optional `Tier`, `Status`, and `Order`
- [ ] 6.3 Enforce the status invariant: `Finished` ⇒ mark (per discipline kind) and place present; otherwise both absent
- [ ] 6.4 Enforce mark-kind match at result construction (time vs distance vs place-only)
- [ ] 6.5 Enforce in `Meet.RecordResult` that the event's sport equals the meet's sport
- [ ] 6.6 Assign `Order` by entry sequence within a student's results for the meet
- [ ] 6.7 Support relay recording: one result per participating student carrying the shared team `TimeMark`, no teammate linkage

## 7. Cross-aggregate domain services

- [ ] 7.T Write failing tests for the level-match service — `Reject a level mismatch` and `Historical meets validate against the season's enrollment` (`record-meet-results` spec) — and for visibility resolution — `Followed students are visible` and `Unrelated students are not visible` (`view-athlete-season` spec), including the `UserAccountId`-match union.
- [ ] 7.1 Implement a domain service that validates a meet's `CompetitionLevel` against the student's enrollment level for the meet's season (resolved by meet date → `SchoolYear`)
- [ ] 7.2 Implement student visibility resolution: `Following` set ∪ the student whose `UserAccountId` equals the user's account

## 8. Derived reads: season view and bests

- [ ] 8.T Write failing tests from `view-athlete-season` and `personal-and-season-best` specs — `Current season is the current enrollment`, `Show a student's current season results` (entry order), `Navigate to a past season`; `Best time wins for a time discipline`, `Best distance wins for a distance discipline`, `Non-finished results are excluded`, `Personal best spans seasons`, `Personal best updates as results are added`, `A JV mark can be a personal best` (tier-agnostic), `Relay marks are excluded`, and `Place-only disciplines have no best`. Use `[Theory]` across multiple seasons and both directions.
- [ ] 8.1 Implement a read projection listing a student's results for a season's `SchoolYear`, ordered by `Order`
- [ ] 8.2 Implement `SeasonBest(student, discipline, season)` — best finished, individual, mark-present result, direction per mark kind
- [ ] 8.3 Implement `PersonalBest(student, discipline)` as the best across season bests
- [ ] 8.4 Ensure bests exclude non-`Finished` results, relays, and place-only disciplines, and are tier-agnostic

## 9. Cleanup

- [ ] 9.1 Delete `docs/narratives/domain-model-design.md` once this change is the source of truth
