## Context

`Trakmark.Domain.Tests` has 160 passing tests but branch coverage is 58.8% (target: 70%). The deficit traces to two root causes:

1. **Missing equality tests** — 91 of 113 flagged uncovered methods are the `Equals(T)`, `Equals(object)`, `GetHashCode`, `==`, `!=`, and `ToString` surface on 14 domain types. These methods exist in production code but no test exercises them.
2. **Missing `TryParse` failure paths** — `MeetId`, `RegisteredUserId`, `SchoolId`, `TeamId` each have a `TryParse` method at 50% branch coverage; the `false`-return path is never hit.

Additionally, all 9 test files sit flat at the project root, while `Trakmark.Domain` is organized into `Aggregates/`, `Catalog/`, `Ids/`, `Services/`, `ValueObjects/`.

## Goals / Non-Goals

**Goals:**
- Reorganize test files into subfolders mirroring the domain project structure.
- Update namespaces to `Trakmark.Domain.Tests.<Subfolder>`.
- Add equality tests covering the missing branches on all 14 affected types.
- Add `TryParse` failure-path tests for the four ID types.
- Push branch coverage to ≥ 70%.

**Non-Goals:**
- No production code changes.
- No new test infrastructure (no base classes, no shared fixtures).
- No coverage tooling changes.

## Decisions

### D1: Subfolder structure mirrors domain project

Map each domain subfolder directly to a matching test subfolder:

```
Trakmark.Domain/          →  Trakmark.Domain.Tests/
  Aggregates/             →    Aggregates/
  Catalog/                →    Catalog/
  Ids/                    →    Ids/
  Services/               →    Services/
  ValueObjects/           →    ValueObjects/
```

Namespaces follow the folder: `Trakmark.Domain.Tests.Catalog`, etc.

**Why over flat layout**: navigation matches the production project; new test files land in an obvious location.

### D2: Extend existing test files rather than creating new ones

Equality tests go into the file that already owns that domain area:

| Tests to add | Target file |
|---|---|
| `Event`, `Discipline`, `Tier`, `HurdleHeight`, `ImplementWeight`, `Placement` equality | `Catalog/DisciplineAndMarkTests.cs` |
| `MeetName`, `PersonName`, `SchoolName`, `TeamName`, `GradeLevel`, `CompetitionLevel`, `Sport` equality | `ValueObjects/ValueObjectTests.cs` |
| `TryParse` failure paths | `Ids/StronglyTypedIdTests.cs` |
| `Enrollment` equality | `Aggregates/EnrollmentTests.cs` (new — no existing home) |

**Why**: cohesion — related tests stay together; avoids parallel file structures.

### D3: One `[Theory]` per type covering all equality branches

Each type needs these cases to achieve full branch coverage on `Equals(T)`:

| Case | Expected |
|---|---|
| Same value / same reference | `true` |
| Different value | `false` |
| `null` | `false` |
| `Equals(object)` with wrong type | `false` |
| `==` and `!=` operators | symmetric with `Equals` |
| `GetHashCode` equal instances produce same hash | true |
| `ToString` returns expected string | spot-check |

For **closed-set singletons** (`Tier`, `GradeLevel`, `CompetitionLevel`, `Sport`, `HurdleHeight`, `ImplementWeight`) the "different value" case uses two different static members (e.g., `Tier.Varsity` vs `Tier.JV`).

For **composite types** (`Event`, `Enrollment`) the "different value" cases cover each field independently to exercise all short-circuit branches.

**Why `[Theory]` over multiple `[Fact]`s**: project convention; avoids near-duplicate test proliferation.

### D4: `TryParse` failure paths use existing invalid-input patterns

`StronglyTypedIdTests.cs` already tests `StudentId.TryParse` with wrong-prefix, out-of-charset, and wrong-length inputs. Extend with one `[Theory]` covering all four ID types (`MeetId`, `RegisteredUserId`, `SchoolId`, `TeamId`) with at least one malformed input each.

## Risks / Trade-offs

- **File move breaks IDE bookmarks / git blame** — unavoidable; `git log --follow` preserves history. Risk is low and accepted.
- **Namespace change requires updating usings** — moving files changes the namespace; all test-internal usings (there are none — tests have no cross-references) and any `using` in the files need updating. Verified: no cross-file references exist.
- **Closed-set singleton equality tests are low-value but necessary** — `GradeLevel.Freshman == GradeLevel.Freshman` is trivially true by reference, but the branch inside `Equals(GradeLevel?)` still needs to execute for coverage. Tests are valid behavior assertions even if the scenarios are simple.
