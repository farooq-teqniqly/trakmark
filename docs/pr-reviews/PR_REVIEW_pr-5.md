# PR #5 Review — test-improvements

**Branch:** `test-improvements` → `main`
**Scope:** Full PR (no prior review file found)
**Reviewed commit:** `6df3892` on branch `test-improvements`

---

## Summary of Findings

| Severity | Tag | Topic | Location | Source |
|----------|-----|-------|----------|--------|
| ~~Medium~~ | ~~[New]~~ | ~~Duplicate `[Theory]` test between two files~~ | ~~`SeasonViewServiceTests.cs:63-87` / `SchoolYearHelperTests.cs:29-53`~~ | ~~Claude~~ | **FIXED** |
| ~~Medium~~ | ~~[New]~~ | ~~`SchoolYearHelperTests` tests `SeasonViewService`, not `SchoolYearHelper`~~ | ~~`SchoolYearHelperTests.cs:49`~~ | ~~Claude~~ | **FIXED** — file deleted; theory moved to `SeasonViewServiceTests` as `SeasonBoundaries_CorrectlyResolveSchoolYearFromMeetDate` |
| ~~Low~~ | ~~[Already raised by: Qodo]~~ | ~~Combined `// Arrange / Act` and `// Act / Assert` comments violate CLAUDE.md rule~~ | ~~~90 occurrences across 20+ test files~~ | ~~Qodo, Claude~~ | **ACCEPTED** — combined form acceptable when a single line covers both phases; splitting would require a spurious intermediate variable |
| ~~Low~~ | ~~[New]~~ | ~~`AllDomainIds_RoundTrip` in `UserAccountIdTests` tests four types unrelated to `UserAccountId`~~ | ~~`UserAccountIdTests.cs:51-63`~~ | ~~Claude~~ | **FIXED** |
| ~~Low~~ | ~~[New]~~ | ~~`MeetDateTests.cs` — `Equality_WorksCorrectly` missing explicit operator assertions~~ | ~~`MeetDateTests.cs:22-31`~~ | ~~Claude~~ | **FIXED** |
| ~~Low~~ | ~~[New]~~ | ~~`CompetitionLevelTests.cs` — `GetHashCode` trivially tests same reference against itself~~ | ~~`CompetitionLevelTests.cs:50-56`~~ | ~~Claude~~ | **FIXED** — HashSet deduplication pattern applied to all 5 singleton types |
| ~~Low~~ | ~~[New]~~ | ~~`GradeLevelTests.cs` — no `GetHashCode` test~~ | ~~`GradeLevelTests.cs`~~ | ~~Claude~~ | **FIXED** — `GetHashCode_UsedInHashSet_DeduplicatesEqualInstances` added |
| ~~Info~~ | ~~[Already raised by: Qodo, Copilot]~~ | ~~`Run-Coverage.ps1` — `$repoRoot` resolves to `.claude/skills`, not repo root~~ | ~~`Run-Coverage.ps1:28`~~ | ~~Qodo, Copilot~~ | **FIXED** — `git rev-parse --show-toplevel` |
| ~~Info~~ | ~~[Already raised by: Qodo, Copilot]~~ | ~~`Run-Coverage.ps1` — `Write-Host` for `COBERTURA_PATH:` is not capturable from stdout~~ | ~~`Run-Coverage.ps1:74`~~ | ~~Qodo, Copilot~~ | **FIXED** — `Write-Output` |
| ~~Info~~ | ~~[Already raised by: Copilot]~~ | ~~`SKILL.md` hard-codes developer-local path `C:\src\my\trakmark\...`~~ | ~~`SKILL.md:24`, `SKILL.md:67`~~ | ~~Copilot~~ | **FIXED** — `git rev-parse --show-toplevel` |
| ~~Info~~ | ~~[New]~~ | ~~`DistanceMark_ToString_IsNonEmpty` and `TimeMark_ToString_IsNonEmpty` only assert non-empty, not the exact format~~ | ~~`DistanceMarkTests.cs:85-95`, `TimeMarkTests.cs:85-95`~~ | ~~Claude~~ | **FIXED** |

---

## Existing Review Comments

| Reviewer | File | Line | Topic | Status |
|----------|------|------|-------|--------|
| Qodo | `MeetNameTests.cs` | 26 | Combined `// Arrange / Act` / `// Act / Assert` comments | **Resolved** — accepted as-is |
| Qodo | `Run-Coverage.ps1` | 40 | `$repoRoot` wrong — resolves to `.claude/skills` parent | **Fixed** |
| Qodo | `Run-Coverage.ps1` | 75 | `COBERTURA_PATH:` output via `Write-Host` not capturable | **Fixed** |
| Copilot | `Run-Coverage.ps1` | 32 | Same `$repoRoot` bug | **Fixed** |
| Copilot | `Run-Coverage.ps1` | 75 | Use `Write-Output` for `COBERTURA_PATH:` | **Fixed** |
| Copilot | `SKILL.md` | 26 | Hard-coded developer path in Step 1 snippet | **Fixed** |
| Copilot | `SKILL.md` | 70 | Hard-coded `C:\src\my\trakmark` path in Step 4 | **Fixed** |

---

## TL;DR

The PR successfully accomplishes its stated goal: splitting monolithic test files into per-type files, adding coverage for new domain paths, and resolving all SonarQube warnings. The domain production fixes are clean. The test additions are solid where the AAA structure is clearly separated. Three findings require action before merge: (1) a literal test duplication between `SchoolYearHelperTests` and `SeasonViewServiceTests`, (2) the widespread combined-AAA comment pattern that violates CLAUDE.md, and (3) the coverage-script `$repoRoot` bug that would silently break any CI environment that clones the repo elsewhere. The remaining findings are lower-priority consistency issues.

---

## Architecture / Data Flow

No production architecture changes. Domain service signatures gained `ArgumentNullException.ThrowIfNull` guards (`SeasonViewService`, `StudentVisibilityService`, `BestMarksService`, `CompetitionLevelMatchService`). `TimeMark` and `DistanceMark` were extracted from the monolithic `Performance.cs` into their own files — correct one-type-per-file alignment. `RegisteredUserId` received a static constructor with `[ExcludeFromCodeCoverage]` to suppress the compiler-generated `.cctor` coverage gap. These production changes are all sound.

---

## File-by-File Notes

### `.claude/skills/coverage-report/Run-Coverage.ps1`

**Line 28 — wrong `$repoRoot` [Info, Already raised by Qodo and Copilot]**

```powershell
$repoRoot = Split-Path $PSScriptRoot -Parent  # resolves to .claude/skills, NOT the repo root
```

The script lives at `.claude/skills/coverage-report/Run-Coverage.ps1`, so `$PSScriptRoot` is `.claude/skills/coverage-report` and its parent is `.claude/skills`. The default `SolutionPath` and `OutputDir` therefore point to paths that do not exist. Callers who always supply `-SolutionPath` and `-OutputDir` will not notice, but the script is unusable with defaults anywhere except the original developer machine.

Fix: use `git -C $PSScriptRoot rev-parse --show-toplevel` to locate the repo root reliably.

```powershell
$repoRoot = & git -C $PSScriptRoot rev-parse --show-toplevel
```

**Line 74 — `Write-Host` for machine-readable output [Info, Already raised by Qodo and Copilot]**

```powershell
Write-Host "COBERTURA_PATH:$($xml.FullName)"
```

`Write-Host` writes to the host information stream (stream 6). Callers reading stdout via `$(& .\Run-Coverage.ps1)` or pipes will not see this line. Replace with `Write-Output`.

```powershell
Write-Output "COBERTURA_PATH:$($xml.FullName)"
```

### `.claude/skills/coverage-report/SKILL.md`

**Lines 24 and 67 — hard-coded local path [Info, Already raised by Copilot]**

Step 1 uses `C:\src\my\trakmark\.claude\skills\coverage-report` and Step 4 uses `C:\src\my\trakmark\docs\...`. These paths are machine-specific and break on any other developer's workstation or CI agent. Steps should derive `$skillDir` from a relative or `git rev-parse`-based path, matching the fix described above.

---

### `Trakmark.Domain.Tests/Services/SchoolYearHelperTests.cs`

**Lines 29–53 — test calls `SeasonViewService`, not `SchoolYearHelper` [Medium, New]**

The class is named `SchoolYearHelperTests` and its `<summary>` doc says it tests `SchoolYearHelper.ToSchoolYear`, but every assertion in the single `[Theory]` is made against `SeasonViewService.GetSeasonResults`. `SchoolYearHelper` is an `internal` helper, so per CLAUDE.md it must be exercised through its public API callers — that part is correct. The problem is the test body is **byte-for-byte identical** to `SeasonViewServiceTests.SchoolYearHelper_ToSchoolYear_CorrectlyResolvesSeasonFromMeetDate` (lines 63–87), down to the `[InlineData]` values. This is a duplicate `[Theory]` covering the same scenario, which CLAUDE.md explicitly prohibits.

Resolution: remove `SchoolYearHelperTests` entirely. The four season-resolution boundary cases are already covered by `SeasonViewServiceTests.SchoolYearHelper_ToSchoolYear_CorrectlyResolvesSeasonFromMeetDate`.

### `Trakmark.Domain.Tests/Services/SeasonViewServiceTests.cs`

**Lines 63–87 — `SchoolYearHelper_ToSchoolYear_CorrectlyResolvesSeasonFromMeetDate` test belongs here [Medium, New — context note]**

This test correctly lives in `SeasonViewServiceTests` (because `SchoolYearHelper` is internal and is exercised through `SeasonViewService`). Once `SchoolYearHelperTests` is removed, rename this test to `SeasonView_SeasonBoundaries_CorrectlyResolvedFromMeetDate` to make the intent clear without referencing the internal helper name.

---

### Combined `// Arrange / Act` and `// Act / Assert` comments — widespread [Low, Already raised by Qodo]

CLAUDE.md requires **separate** `// Arrange`, `// Act`, `// Assert` sections in every test. The new test files use collapsed combined forms extensively. The table below lists every affected file and the first offending line.

| File | First combined comment | Occurrences |
|------|------------------------|-------------|
| `ValueObjects/MeetNameTests.cs` | line 13 (`// Arrange / Act`) | 6 |
| `ValueObjects/PersonNameTests.cs` | line 14 | 6 |
| `ValueObjects/SchoolNameTests.cs` | line 13 | 6 |
| `ValueObjects/TeamNameTests.cs` | line 13 | 6 |
| `ValueObjects/CompetitionLevelTests.cs` | line 25 | 5 |
| `ValueObjects/GradeLevelTests.cs` | line 11 | 5 |
| `ValueObjects/SchoolYearTests.cs` | line 24 | 2 |
| `ValueObjects/SportTests.cs` | line 24 | 4 |
| `Catalog/DisciplineTests.cs` | line 15 | 12 |
| `Catalog/DistanceMarkTests.cs` | line 15 | 6 |
| `Catalog/EventTests.cs` | line 28 | 4 |
| `Catalog/HurdleHeightTests.cs` | line 17 | 5 |
| `Catalog/ImplementWeightTests.cs` | line 17 | 5 |
| `Catalog/MarkKindTests.cs` | line 17 | 6 |
| `Catalog/PerformanceTests.cs` | line 19 | 1 |
| `Catalog/PlacementTests.cs` | line 17 | 6 |
| `Catalog/TierTests.cs` | line 17 | 5 |
| `Catalog/TimeMarkTests.cs` | line 15 | 6 |
| `Services/SeasonViewServiceTests.cs` | line 47 | 2 |
| `Aggregates/EnrollmentTests.cs` | line 37 | 4 |
| `Aggregates/MeetResultTests.cs` | line 32 | 10 |
| `Aggregates/SchoolAggregateTests.cs` | line 38 | 2 |
| `Ids/DomainIdFormatTests.cs` | line 50 | 3 |
| `Ids/MeetIdTests.cs` | line 15 | 3 |
| `Ids/RegisteredUserIdTests.cs` | line 15 | 3 |
| `Ids/SchoolIdTests.cs` | line 15 | 3 |
| `Ids/StudentIdTests.cs` | line 11 | 4 |
| `Ids/TeamIdTests.cs` | line 15 | 3 |
| `Ids/UserAccountIdTests.cs` | line 27 | 2 |

The fix for tests where arrange and act are inseparable (e.g., a single-expression constructor call that is both the setup and the operation): place the assignment on the Arrange line and the assertion alone under Assert. Where the test truly has no separate Arrange phase, use `// Arrange` with a `// (none)` note or restructure so the subject is introduced under Arrange.

Example (before):
```csharp
// Arrange / Act / Assert
Assert.Throws<ArgumentException>(() => new MeetName(input));
```

Example (after):
```csharp
// Arrange — input provided by [InlineData]
// Act / Assert
Assert.Throws<ArgumentException>(() => new MeetName(input));
```

Note: some tests use `// Arrange / Act` where the construction IS the act. For those, split into:
```csharp
// Arrange — (value provided by [InlineData])
// Act
var name = new MeetName(input);
// Assert
Assert.Equal(input.Trim(), name.Value);
```

---

### `Trakmark.Domain.Tests/Ids/UserAccountIdTests.cs`

**Lines 51–63 — `AllDomainIds_RoundTrip` belongs in per-type ID test files [Low, New]**

```csharp
public void AllDomainIds_RoundTrip()   // line 51
{
    var meetId   = MeetId.NewId();
    var schoolId = SchoolId.NewId();
    var teamId   = TeamId.NewId();
    var userId   = RegisteredUserId.NewId();
    ...
}
```

`UserAccountIdTests` is the test file for `UserAccountId`, not a cross-type integration harness. Per CLAUDE.md "one test class per production type, one file per test class", this test should not live here. Each of `MeetIdTests`, `SchoolIdTests`, `TeamIdTests`, and `RegisteredUserIdTests` already has a `_RoundTrips` `[Fact]`. The combined round-trip test in `UserAccountIdTests` is surplus and tests the wrong type.

Resolution: remove `AllDomainIds_RoundTrip` from `UserAccountIdTests`.

---

### `Trakmark.Domain.Tests/ValueObjects/MeetDateTests.cs`

**Lines 22–31 — `Equality_WorksCorrectly` missing operator coverage [Low, New]**

The `Equality_WorksCorrectly` test at line 22 only calls `Assert.Equal(a, b)` (which calls `Object.Equals`). `MeetDate` is a `readonly record struct`; its auto-generated `==` and `!=` are exercised nowhere in this file. The broader project standard requires `==`/`!=` to be verified. Add:

```csharp
Assert.True(a == b);
Assert.False(a != b);
```

---

### `Trakmark.Domain.Tests/ValueObjects/CompetitionLevelTests.cs`

**Lines 50–56 — `GetHashCode_EqualInstances_SameHash` tests the same reference twice [Low, New]**

```csharp
Assert.Equal(
    CompetitionLevel.HighSchool.GetHashCode(),
    CompetitionLevel.HighSchool.GetHashCode());    // same reference both sides
```

Because `CompetitionLevel.HighSchool` is a singleton, calling `GetHashCode()` twice on it proves nothing — it will always be equal. The pattern used elsewhere (e.g., `PlacementTests.cs:50-55`, `CompetitionLevelTests.cs:60-71`) should be followed: verify that **two independently obtained instances** that are equal produce the same hash. For a singleton type, the `GetHashCode_UsedInHashSet_DeduplicatesEqualInstances` test at line 59 already provides meaningful coverage; the hash test at line 50 can be removed or replaced with a two-singleton comparison.

Same issue appears in:
- `Catalog/HurdleHeightTests.cs:44-45`
- `Catalog/ImplementWeightTests.cs:44-45`
- `Catalog/MarkKindTests.cs:44-45`
- `Catalog/TierTests.cs:44-45`

---

### `Trakmark.Domain.Tests/ValueObjects/GradeLevelTests.cs`

**Missing `GetHashCode` test [Low, New]**

`GradeLevelTests` covers `Equals`, null-guard, wrong-type, and `ToString` but has no `GetHashCode` assertion. Every other closed-set value object in this PR (`Sport`, `CompetitionLevel`, `HurdleHeight`, `Tier`, `ImplementWeight`, `MarkKind`) includes one. Add either a trivial same-reference hash test or a `HashSet<GradeLevel>` deduplication test consistent with the `CompetitionLevelTests` pattern.

---

### `Trakmark.Domain.Tests/Catalog/DistanceMarkTests.cs` and `TimeMarkTests.cs`

**`ToString` tests only assert non-empty [Info, New]**

```csharp
// DistanceMarkTests.cs:93
Assert.NotEmpty(result);

// TimeMarkTests.cs:93
Assert.NotEmpty(result);
```

The production code at `DistanceMark.cs:44` and `TimeMark.cs:44` formats as `"{value}cm"` and `"{value}ms"` respectively. The tests should assert the exact format:

```csharp
// DistanceMark
Assert.Equal("600cm", result);

// TimeMark
Assert.Equal("11500ms", result);
```

This converts a test that can never fail into a regression guard.

---

## Checklist

### Functionality
- [x] Core functionality works as intended
- [x] Edge cases handled
- [x] Error scenarios covered — null guards added to all domain services
- [x] Configuration correct

### Thread Safety
- [x] No new shared mutable state introduced
- [x] Static service methods are stateless

### Performance
- [x] No unnecessary allocations in domain services
- [x] Coverage script uses `-Clean` flag correctly

### Code Quality
- [x] Follows existing patterns (sealed classes, ArgumentNullException.ThrowIfNull)
- [x] Proper error handling and logging (no new logger calls in this PR)
- [x] SonarQube warnings resolved with targeted `#pragma disable` where unavoidable
- [x] XML `<summary>` docs present on all public/internal members in production code
- [x] ~~Combined AAA comments in test files~~ **ACCEPTED** — combined form acceptable when Arrange and Act are a single line
- [ ] Cyclomatic complexity ≤ 15 — not at risk in this PR

### Simplification and Refactoring
- [x] ~~Duplicate `[Theory]` between `SchoolYearHelperTests` and `SeasonViewServiceTests`~~ **FIXED**
- [x] ~~`AllDomainIds_RoundTrip` placed in the wrong test class~~ **FIXED**
- [x] ~~Tautological `GetHashCode` tests on singleton types (same reference both sides)~~ **FIXED**

### Testing
- [x] Follows project patterns (xUnit, NSubstitute where needed)
- [x] One test class per production type
- [x] One file per test class
- [x] ~~`[Theory]` duplication between `SchoolYearHelperTests` and `SeasonViewServiceTests`~~ **FIXED**
- [x] ~~`MeetDateTests` missing `==`/`!=` operator assertions~~ **FIXED**
- [x] ~~`GradeLevelTests` missing `GetHashCode` test~~ **FIXED**
- [x] ~~`ToString` tests for `TimeMark` and `DistanceMark` assert only non-empty, not format~~ **FIXED**

---

## Questions for the Author

1. `SchoolYearHelperTests` was added as a new file — was the intent to eventually test `SchoolYearHelper` directly (which is `internal` and therefore prohibited by CLAUDE.md), or was this file intended to host the season-boundary tests and the production class name crept in?

2. The combined `// Arrange / Act / Assert` comments appear throughout every new test file. Was this a deliberate style choice for compact one-liner assertion tests, or an oversight given the CLAUDE.md requirement for separate sections?

3. `MeetId.TryParse` in `MeetIdTests.cs` uses `"MEET-7F3K90"` as a malformed input (line 11) — this input has a `0` in the body, which is not in the Crockford charset. But the first column of the `TryParse` `[Theory]` also includes `"SCH-7F3K90"` (wrong prefix), not `"MEET-7F3K90"`. Was the intent to test the wrong-prefix path or the invalid-charset path? Currently both are tested but the naming suggests only one was intended.

---

## Process Improvements

| Pattern | Files affected | Suggested fix |
|---------|----------------|---------------|
| Combined `// Arrange / Act` and `// Act / Assert` comments across all 20+ new test files, indicating the three-section rule was not consulted during authoring | 20+ test files, ~90 occurrences | Add a self-check step to `CLAUDE.md` under the test conventions section: "Before committing a test file, scan every method for combined AAA comments (`// Arrange / Act`, `// Act / Assert`) and replace with three separate sections." Also add a note to `openspec/config.yaml` `rules.tasks` so the TDD session reminders include the AAA check. |
| Tautological singleton hash tests (same reference compared to itself) across 5 catalog test files | `CompetitionLevelTests.cs`, `HurdleHeightTests.cs`, `ImplementWeightTests.cs`, `MarkKindTests.cs`, `TierTests.cs` | Add a CLAUDE.md example under the equality testing guidance: for singleton value objects, the `GetHashCode` test must compare **two different singleton instances** or use a `HashSet` deduplication test. The tautological form (same reference vs. same reference) must not be used. |
| `ToString` tests asserting only `Assert.NotEmpty` in `TimeMark` and `DistanceMark`, providing no format regression protection | `DistanceMarkTests.cs`, `TimeMarkTests.cs` | Add to CLAUDE.md test conventions: "`ToString` tests must assert the exact expected string, not merely `NotEmpty` — `NotEmpty` can never fail on a non-null type with a real override." |

---

## References

- CLAUDE.md — test conventions (AAA structure, Theory over duplicate Facts, one class per file)
- Qodo review comment on `MeetNameTests.cs:26` — combined AAA
- Qodo review comment on `Run-Coverage.ps1:40` — wrong `$repoRoot`
- Copilot review comment on `Run-Coverage.ps1:75` — `Write-Host` vs `Write-Output`
- Copilot review comment on `SKILL.md:26`, `SKILL.md:70` — hard-coded developer paths
