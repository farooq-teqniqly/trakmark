## Why

Branch coverage in `Trakmark.Domain.Tests` sits at 58.8% against a 70% target. The gap is driven almost entirely by untested equality and operator boilerplate on value objects and catalog types, plus missing `TryParse` failure paths on ID types. The test files are also flat in the project root, making navigation harder as the suite grows.

## What Changes

- Reorganize all existing test files into subfolders mirroring `Trakmark.Domain` (`Aggregates/`, `Catalog/`, `Ids/`, `Services/`, `ValueObjects/`); update namespaces to match (`Trakmark.Domain.Tests.<Subfolder>`).
- Add equality tests (`Equals(T)`, `Equals(object)`, `GetHashCode`, `==`, `!=`, `ToString`) for all value objects and catalog types with zero-coverage equality surfaces: `MeetName`, `PersonName`, `SchoolName`, `TeamName`, `GradeLevel`, `CompetitionLevel`, `Sport`, `Tier`, `HurdleHeight`, `ImplementWeight`, `Placement`, `Discipline`, `Event`, `Enrollment`.
- Add `TryParse` failure-path tests for `MeetId`, `RegisteredUserId`, `SchoolId`, `TeamId`.

## Capabilities

### New Capabilities

None — this change adds tests and reorganizes test files; no production capabilities are introduced.

### Modified Capabilities

None — no spec-level behavior changes.

## Impact

- `Trakmark.Domain.Tests`: all `.cs` files move into subfolders; namespaces change from `Trakmark.Domain.Tests` to `Trakmark.Domain.Tests.<Subfolder>`.
- Branch coverage expected to rise from 58.8% to ≥ 70%.
- `Event.Equals` CRAP score drops from 20 to ~4.
- No production code changes.
