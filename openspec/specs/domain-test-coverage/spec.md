## Requirements

### Requirement: Value object equality is fully tested

The test suite SHALL exercise the complete equality surface (`Equals(T)`, `Equals(object)`, `GetHashCode`, `==`, `!=`) for every value object and catalog type that implements `IEquatable<T>`. Covered types: `MeetName`, `PersonName`, `SchoolName`, `TeamName`, `GradeLevel`, `CompetitionLevel`, `Sport`, `Tier`, `HurdleHeight`, `ImplementWeight`, `Placement`, `Discipline`, `Event`, `Enrollment`.

#### Scenario: Equal instances compare as equal

- **WHEN** two instances hold the same value (or the same closed-set singleton is compared to itself)
- **THEN** `Equals(T)` returns `true`, `==` returns `true`, `!=` returns `false`, and both produce the same `GetHashCode`

#### Scenario: Unequal instances compare as unequal

- **WHEN** two instances hold different values (or two different closed-set singletons are compared)
- **THEN** `Equals(T)` returns `false`, `==` returns `false`, `!=` returns `true`

#### Scenario: Null comparison returns false

- **WHEN** `Equals(T)` or `==` is called with a `null` argument
- **THEN** the result is `false`

#### Scenario: Wrong-type object comparison returns false

- **WHEN** `Equals(object)` is called with an object of an incompatible type
- **THEN** the result is `false`

#### Scenario: Event equality exercises each field independently

- **WHEN** two `Event` instances share the same `Discipline` but differ in `Sport` (or vice versa)
- **THEN** `Equals` returns `false`, exercising the short-circuit branch for the differing field

#### Scenario: Enrollment equality exercises each field independently

- **WHEN** two `Enrollment` instances share the same `SchoolId` and `SchoolYear` but differ in `GradeLevel` (or vary other fields)
- **THEN** `Equals` returns `false`, exercising each branch independently

### Requirement: ID TryParse failure paths are tested

The test suite SHALL exercise the `false`-return path of `TryParse` for `MeetId`, `RegisteredUserId`, `SchoolId`, and `TeamId` with at least one malformed input each.

#### Scenario: TryParse returns false for a malformed ID

- **WHEN** `TryParse` is called with a string that has the wrong prefix, wrong body length, or invalid Crockford characters
- **THEN** `TryParse` returns `false` and the out parameter is `default`

### Requirement: DistanceMark and TimeMark equality is fully tested

The test suite SHALL exercise the complete equality surface (`Equals(T)`, `Equals(object)`, `GetHashCode`, `==`, `!=`, `ToString`) for `DistanceMark` and `TimeMark`. Both types are currently at 0% coverage on all equality members.

#### Scenario: Equal DistanceMark instances compare as equal

- **WHEN** two `DistanceMark` instances hold the same distance value
- **THEN** `Equals(T)` returns `true`, `==` returns `true`, `!=` returns `false`, and both produce the same `GetHashCode`

#### Scenario: Unequal DistanceMark instances compare as unequal

- **WHEN** two `DistanceMark` instances hold different distance values
- **THEN** `Equals(T)` returns `false`, `==` returns `false`, `!=` returns `true`

#### Scenario: DistanceMark null and wrong-type comparisons return false

- **WHEN** `Equals(T)` is called with `null`, or `Equals(object)` is called with an incompatible type
- **THEN** the result is `false`

#### Scenario: DistanceMark ToString returns formatted value

- **WHEN** `ToString()` is called on a `DistanceMark`
- **THEN** the result is a non-null, non-empty string representation of the distance

#### Scenario: Equal TimeMark instances compare as equal

- **WHEN** two `TimeMark` instances hold the same time value
- **THEN** `Equals(T)` returns `true`, `==` returns `true`, `!=` returns `false`, and both produce the same `GetHashCode`

#### Scenario: Unequal TimeMark instances compare as unequal

- **WHEN** two `TimeMark` instances hold different time values
- **THEN** `Equals(T)` returns `false`, `==` returns `false`, `!=` returns `true`

#### Scenario: TimeMark null and wrong-type comparisons return false

- **WHEN** `Equals(T)` is called with `null`, or `Equals(object)` is called with an incompatible type
- **THEN** the result is `false`

#### Scenario: TimeMark ToString returns formatted value

- **WHEN** `ToString()` is called on a `TimeMark`
- **THEN** the result is a non-null, non-empty string representation of the time

### Requirement: BestMarksService.PersonalBest branch coverage is complete

The test suite SHALL exercise all branches in `BestMarksService.PersonalBest`, which currently has 60% branch coverage (CRAP score 10). Uncovered branches include: empty result set, multiple disciplines where none match the candidate, and a candidate where `IsBetterThan` returns `false` for every entry.

#### Scenario: PersonalBest returns null for empty results

- **WHEN** `PersonalBest` is called with an empty collection of results
- **THEN** the return value is `null` (or equivalent empty indicator)

#### Scenario: PersonalBest returns null when no results match the discipline

- **WHEN** `PersonalBest` is called with results covering disciplines other than the requested one
- **THEN** the return value is `null`

#### Scenario: PersonalBest returns null when no result is better than the reference

- **WHEN** `PersonalBest` is called and `IsBetterThan` returns `false` for every candidate
- **THEN** the return value is `null`

### Requirement: SchoolYear comparison operators are tested

The test suite SHALL exercise `op_GreaterThanOrEqual` and `op_LessThanOrEqual` on `SchoolYear`, both currently at 0% coverage.

#### Scenario: SchoolYear comparison operators return correct results at boundary

- **WHEN** two `SchoolYear` instances are compared with `>=` or `<=`
- **THEN** equal years satisfy both `>=` and `<=`; a later year satisfies `>` and `>=` relative to an earlier year; an earlier year satisfies `<` and `<=` relative to a later year

### Requirement: CompetitionLevelMatchService.IsLevelMatch branch coverage is complete

The test suite SHALL exercise all branches in `CompetitionLevelMatchService.IsLevelMatch`, which currently has 75% branch coverage (1/4 branches uncovered). The missing branch is the school-level lookup path.

#### Scenario: IsLevelMatch covers the school-level lookup branch

- **WHEN** `IsLevelMatch` is called with inputs that exercise the school-level lookup path
- **THEN** the method returns the correct result and the branch is covered

### Requirement: Remaining catalog boilerplate equality is tested

The test suite SHALL exercise `Equals(object)`, `GetHashCode`, and `ToString` on the remaining catalog types with zero-coverage boilerplate: `Event`, `HurdleHeight`, `ImplementWeight`, `MarkKind`, `Placement`, and `Tier`.

#### Scenario: Catalog type object equality returns false for wrong type

- **WHEN** `Equals(object)` is called on a catalog type with an argument of an incompatible type
- **THEN** the result is `false`

#### Scenario: Catalog type GetHashCode is consistent with equality

- **WHEN** `GetHashCode` is called on two equal catalog type instances
- **THEN** both return the same hash code

#### Scenario: Catalog type ToString returns a non-empty string

- **WHEN** `ToString()` is called on a catalog type instance
- **THEN** the result is a non-null, non-empty string

### Requirement: DomainId.IsValid branch coverage is complete

The test suite SHALL exercise all branches in `DomainId.IsValid`, which currently has 83.3% branch coverage (1/6 branches uncovered). The missing branch is likely the null or empty-string input path.

#### Scenario: IsValid returns false for null or empty input

- **WHEN** `IsValid` is called with a `null` or empty string
- **THEN** the result is `false`

### Requirement: Meet.EnforceFinishedInvariant is fully covered

The test suite SHALL exercise the missing branch in `Meet.EnforceFinishedInvariant` (currently 94.4% line, CRAP 8.01). The uncovered line corresponds to a specific Event/Performance/Placement combination that the existing tests do not reach.

#### Scenario: EnforceFinishedInvariant rejects the uncovered input combination

- **WHEN** `EnforceFinishedInvariant` is called with the specific Event/Performance/Placement combination that exercises the missing branch
- **THEN** the method enforces the invariant correctly (throws or returns as designed) and the branch is covered

### Requirement: ID Parse failure paths are tested

The test suite SHALL exercise the throw/failure path of `Parse` for `MeetId`, `RegisteredUserId`, `SchoolId`, and `TeamId`. Each `Parse` method is currently at 50% branch coverage — the invalid-input branch is never exercised.

#### Scenario: Parse throws for an invalid input

- **WHEN** `Parse` is called with a malformed or invalid string on any of `MeetId`, `RegisteredUserId`, `SchoolId`, or `TeamId`
- **THEN** the method throws the expected exception

### Requirement: IsBetterThan equal/worse branch is tested for performance mark types

The test suite SHALL exercise the equal-or-worse branch of `IsBetterThan` for `DistanceMark` and `TimeMark`. Both are currently at 50% branch coverage — only the "better" path is exercised.

#### Scenario: IsBetterThan returns false when mark is equal or worse

- **WHEN** `IsBetterThan` is called on a `DistanceMark` or `TimeMark` with a candidate that is equal to or worse than the reference
- **THEN** the result is `false`

### Requirement: Test files are organized in subfolders mirroring the domain project

The test project SHALL organize test files into subfolders matching `Trakmark.Domain`'s structure (`Aggregates/`, `Catalog/`, `Ids/`, `Services/`, `ValueObjects/`). Namespaces SHALL follow the folder: `Trakmark.Domain.Tests.<Subfolder>`.

#### Scenario: Test file location matches domain area

- **WHEN** a test class covers types from `Trakmark.Domain.<Area>`
- **THEN** the test file lives under `Trakmark.Domain.Tests/<Area>/` and uses namespace `Trakmark.Domain.Tests.<Area>`
