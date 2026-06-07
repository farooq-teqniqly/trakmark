## ADDED Requirements

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

### Requirement: Test files are organized in subfolders mirroring the domain project

The test project SHALL organize test files into subfolders matching `Trakmark.Domain`'s structure (`Aggregates/`, `Catalog/`, `Ids/`, `Services/`, `ValueObjects/`). Namespaces SHALL follow the folder: `Trakmark.Domain.Tests.<Subfolder>`.

#### Scenario: Test file location matches domain area

- **WHEN** a test class covers types from `Trakmark.Domain.<Area>`
- **THEN** the test file lives under `Trakmark.Domain.Tests/<Area>/` and uses namespace `Trakmark.Domain.Tests.<Area>`
