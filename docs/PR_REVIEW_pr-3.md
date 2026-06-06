# PR #3 Review — feat: domain model pilot — aggregates, services, derived reads

**Branch:** `domain-modelling` → `main`
**Reviewed commit:** f546bbd on branch domain-modelling
**Scope:** 78 files changed, +6116 / -9 lines
**Date:** 2026-06-06

---

## Summary of Findings

| Severity | Tag | Topic | Location | Source |
|---|---|---|---|---|
| ~~High~~ | ~~[Resolved in f546bbd]~~ | ~~Non-thread-safe `Random` field~~ | ~~`Trakmark.Domain/Ids/CrockfordBase32.cs:12`~~ | ~~Copilot, Claude~~ |
| ~~High~~ | ~~[Resolved in b4eeea3]~~ | ~~`DomainId.IsValid` does not guard against `null` input~~ | ~~`Trakmark.Domain/Ids/DomainId.cs:15-18`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing `ArgumentNullException.ThrowIfNull` in `UserAccountId` constructor~~ | ~~`Trakmark.Domain/Ids/UserAccountId.cs:13`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `Event` constructor~~ | ~~`Trakmark.Domain/Catalog/Event.cs:24-28`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `Discipline` factory methods~~ | ~~`Trakmark.Domain/Catalog/Discipline.cs:49,58,66,73`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `Student` constructor and `AddEnrollment`~~ | ~~`Trakmark.Domain/Aggregates/Student.cs:28,39`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `RegisteredUser.AddStudent`~~ | ~~`Trakmark.Domain/Aggregates/RegisteredUser.cs:52`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `School.Create`~~ | ~~`Trakmark.Domain/Aggregates/School.cs:41`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~Missing null guards on `Meet.Create` and `Meet.RecordResult`~~ | ~~`Trakmark.Domain/Aggregates/Meet.cs:53,79`~~ | ~~Copilot, Claude~~ |
| ~~Medium~~ | ~~[Resolved in b7a0226]~~ | ~~`Enrollment` constructor missing null guard on `gradeLevel`~~ | ~~`Trakmark.Domain/Aggregates/Enrollment.cs:23`~~ | ~~Claude~~ |
| ~~Medium~~ | ~~[Resolved in bc756b3]~~ | ~~`GradeLevel` equality broken: `ReferenceEquals` but `GetHashCode` uses `Name`~~ | ~~`Trakmark.Domain/ValueObjects/GradeLevel.cs:36,42`~~ | ~~Claude~~ |
| ~~Medium~~ | ~~[Resolved in bc756b3]~~ | ~~`MarkKind`, `CompetitionLevel`, `Sport` — same `ReferenceEquals`/name-hash inconsistency~~ | ~~`Trakmark.Domain/Catalog/MarkKind.cs:31,37` `Trakmark.Domain/ValueObjects/CompetitionLevel.cs:24,30` `Trakmark.Domain/ValueObjects/Sport.cs:21,27`~~ | ~~Claude~~ |
| ~~Medium~~ | ~~[Resolved in a72ce5d]~~ | ~~`Performance` base class is `public abstract` (not `sealed`); subtypes should use `==`/`!=` operators~~ | ~~`Trakmark.Domain/Catalog/Performance.cs:8`~~ | ~~Claude~~ |
| Medium | [New] | `Career.TryAdd` uses O(n) linear scan before binary-search insert | `Trakmark.Domain/Aggregates/Career.cs:57` | Claude |
| ~~Low~~ | ~~[Resolved in 61f73b5]~~ | ~~`Discipline` factory methods missing `==`/`!=` operator overloads~~ | ~~`Trakmark.Domain/Catalog/Discipline.cs:76-92`~~ | ~~Claude~~ |
| ~~Low~~ | ~~[Resolved in 61f73b5]~~ | ~~`Placement`, `TimeMark`, `DistanceMark` missing `==`/`!=` operator overloads~~ | ~~`Trakmark.Domain/Catalog/Performance.cs:21-91` `Trakmark.Domain/Catalog/Placement.cs:8`~~ | ~~Claude~~ |
| ~~Low~~ | ~~[Resolved in 61f73b5]~~ | ~~`Event` missing `==`/`!=` operator overloads~~ | ~~`Trakmark.Domain/Catalog/Event.cs:10`~~ | ~~Claude~~ |
| ~~Low~~ | ~~[Resolved in 2d41c04]~~ | ~~`Enrollment` missing `<summary>` doc on `GradeLevel` constructor parameter~~ | ~~`Trakmark.Domain/Aggregates/Enrollment.cs:22`~~ | ~~Claude~~ |
| Low | [New] | `SeasonViewService.GetSeasonResults` returns deferred LINQ; callers may double-enumerate | `Trakmark.Domain/Services/SeasonViewService.cs:28` | Claude |
| Low | [New] | `StudentVisibilityService` — `isLinked` compares `UserAccountId?` with `UserAccountId` without null check | `Trakmark.Domain/Services/StudentVisibilityService.cs:39` | Claude |
| ~~Low~~ | ~~[Resolved in c47b495]~~ | ~~`Career` public `AddEnrollment` duplicates logic of `TryAdd`~~ | ~~`Trakmark.Domain/Aggregates/Career.cs:41-48`~~ | ~~Claude~~ |
| ~~Info~~ | ~~[N/A — FluentAssertions will not be used]~~ | ~~Test project missing `FluentAssertions` / `bUnit` (domain-only, acceptable)~~ | ~~`Trakmark.Domain.Tests/`~~ | ~~Claude~~ |
| ~~Info~~ | ~~[Resolved in 82f90b6]~~ | ~~No `==`/`!=` operators on closed-set types used in `Dictionary` keys (`Sport`)~~ | ~~`Trakmark.Domain/ValueObjects/Sport.cs`~~ | ~~Claude~~ |
| ~~Info~~ | ~~[Resolved in 0aad342]~~ | ~~`Result` internal constructor: `Event` and `Tier` reference parameters not null-guarded~~ | ~~`Trakmark.Domain/Aggregates/Result.cs:60`~~ | ~~Claude~~ |

---

## Existing Review Comments (deduplicated index)

All 20 Copilot inline review comments are from a single review pass (review `4441355314`). No human inline comments. One Qodo summary issue-level comment. No comments have been addressed in code yet.

| ID | Source | Path:Line | Topic |
|---|---|---|---|
| ~~PRRC_kwDOSpaLns7IqDwi~~ | ~~Copilot~~ | ~~`CrockfordBase32.cs:13`~~ | ~~Non-thread-safe `Random`~~ |
| ~~PRRC_kwDOSpaLns7IqDwq~~ | ~~Copilot~~ | ~~`DomainId.cs:19`~~ | ~~No null guard → NRE instead of `false`~~ |
| ~~PRRC_kwDOSpaLns7IqDwv~~ | ~~Copilot~~ | ~~`UserAccountId.cs:14`~~ | ~~Missing `ThrowIfNull` on constructor~~ |
| ~~PRRC_kwDOSpaLns7IqDwz~~ | ~~Copilot~~ | ~~`Event.cs:28`~~ | ~~Missing `ThrowIfNull` on constructor~~ |
| ~~PRRC_kwDOSpaLns7IqDw3~~ | ~~Copilot~~ | ~~`Discipline.cs:52`~~ | ~~Missing `ThrowIfNull` on `HurdleRun`~~ |
| ~~PRRC_kwDOSpaLns7IqDw6~~ | ~~Copilot~~ | ~~`Discipline.cs:60`~~ | ~~Missing `ThrowIfNull` on `ImplementThrow`~~ |
| ~~PRRC_kwDOSpaLns7IqDxE~~ | ~~Copilot~~ | ~~`Discipline.cs:67`~~ | ~~Missing `ThrowIfNull` on `Jump`~~ |
| ~~PRRC_kwDOSpaLns7IqDxQ~~ | ~~Copilot~~ | ~~`Discipline.cs:74`~~ | ~~Missing `ThrowIfNull` on `PlaceOnly`~~ |
| ~~PRRC_kwDOSpaLns7IqDxd~~ | ~~Copilot~~ | ~~`Student.cs:33`~~ | ~~Missing `ThrowIfNull` on constructor~~ |
| ~~PRRC_kwDOSpaLns7IqDxm~~ | ~~Copilot~~ | ~~`Student.cs:43`~~ | ~~Missing `ThrowIfNull` on `AddEnrollment`~~ |
| ~~PRRC_kwDOSpaLns7IqDxx~~ | ~~Copilot~~ | ~~`RegisteredUser.cs:56`~~ | ~~Missing `ThrowIfNull` on `AddStudent`~~ |
| ~~PRRC_kwDOSpaLns7IqDx9~~ | ~~Copilot~~ | ~~`School.cs:42`~~ | ~~Missing `ThrowIfNull` on `Create`~~ |
| ~~PRRC_kwDOSpaLns7IqDyI~~ | ~~Copilot~~ | ~~`Meet.cs:54`~~ | ~~Missing `ThrowIfNull` on `Create`~~ |
| ~~PRRC_kwDOSpaLns7IqDyS~~ | ~~Copilot~~ | ~~`Meet.cs:~87`~~ | ~~Missing `ThrowIfNull` on `RecordResult`~~ |
| ~~PRRT_kwDOSpaLns6HhmyW~~ | ~~Copilot~~ | ~~`OperationResult.cs:25`~~ | ~~Missing `ThrowIfNull` on `Failure` — N/A: `reason` is `string?`, null guard does not apply~~ |

---

## TL;DR

This is a well-structured, TDD-driven domain model pilot: 160 tests, 44 spec scenarios covered, clean DDD aggregate design, strong XML documentation, correct use of `sealed` types and closed-set patterns. All previously identified blocking concerns and null-guard sweep items have been resolved; remaining open findings are Medium or below.

~~**Block 1 (High):** `CrockfordBase32.Random` is `new Random()` (not thread-safe). Under concurrent ID generation from multiple threads, this can produce corrupted/identical sequences. Replace with `Random.Shared`.~~ ✓ Resolved

~~**Block 2 (Medium, widespread):** The null-guard convention from CLAUDE.md (`ArgumentNullException.ThrowIfNull` on every public constructor/method reference parameter) is missing across the majority of public API methods — exactly as Copilot flagged. The pattern is applied correctly in `BestMarksService`, `SeasonViewService`, `CompetitionLevelMatchService`, and `StudentVisibilityService`, but not in the lower-level aggregates and catalog types. A single sweep pass is needed.~~ ✓ Resolved

~~**Design concern (Medium):** The closed-set types (`GradeLevel`, `MarkKind`, `CompetitionLevel`, `Sport`) use `ReferenceEquals` for `Equals` but use `Name.GetHashCode(...)` for `GetHashCode`. CLAUDE.md explicitly requires these to use the same comparer. For singletons accessed only via static fields this is benign today, but violates the rule and breaks if any instance is ever deserialized or created via reflection (e.g., by EF Core). Fix: either use `RuntimeHelpers.GetHashCode(this)` (identity hash) or switch both `Equals` and `GetHashCode` to name-based comparison.~~ ✓ Resolved

---

## Architecture and Data Flow

The architecture is clean and appropriate for a persistence-ignorant domain layer:

- **Strongly-typed IDs** (`StudentId`, `MeetId`, `SchoolId`, `TeamId`, `RegisteredUserId`): `readonly record struct` with `PREFIX-BODY(6)` Crockford base32 format. Shared generation/validation logic in `DomainId` + `CrockfordBase32`.
- **Aggregates**: `Student`, `Meet`, `School`, `RegisteredUser` as aggregate roots. `Career`, `Enrollment`, `Result` as owned entities/value objects within those roots.
- **Catalog types**: `Discipline`, `Event`, `Performance` (with `TimeMark`/`DistanceMark`), `MarkKind`, `Tier`, `HurdleHeight`, `ImplementWeight`, `Placement` — stateless or closed-set singletons.
- **Domain services**: Five services handle cross-aggregate invariants. All are stateless, `sealed`, well-documented, and properly null-guard their public parameters.
- **Derived reads**: `SeasonViewService` and `BestMarksService` project from the result list — correctly derived, never cached.

The decision to denormalize `MeetDate` onto `Result` (so season-view projections do not require a join back to the owning `Meet`) is clearly documented and pragmatically correct for a persistence-ignorant model.

`Career` uses binary search for insertion (O(log n)) but a full linear scan to check for duplicate years before inserting. This could be a single O(log n) operation (see findings below).

---

## File-by-File Notes

### `Trakmark.Domain/Ids/CrockfordBase32.cs`

**Lines 12-20 — Non-thread-safe `Random` [High]**

`private static readonly Random Random = new();` is an instance of the non-thread-safe `System.Random`. Under concurrent ID generation (e.g., two parallel requests each calling `StudentId.NewId()`), calling `Random.Next` from multiple threads can corrupt the internal state, causing all subsequent calls to return 0 or the same value.

Fix: use `Random.Shared` (thread-safe, .NET 6+):

```csharp
// Before
private static readonly Random Random = new();
chars[i] = Alphabet[Random.Next(Alphabet.Length)];

// After — remove the field entirely
chars[i] = Alphabet[Random.Shared.Next(Alphabet.Length)];
```

**Line 30 — `IndexOf` for character validation**

`Alphabet.IndexOf(c)` on a `const string` performs a O(n) scan per character. For the 6-character ID body this is negligible, but a `HashSet<char>` or a pre-built `bool[]` array indexed by char would be more idiomatic in a hot path. Info-level only given the small body length.

---

### `Trakmark.Domain/Ids/DomainId.cs`

**Lines 15-18 — No null guard on `IsValid` [High]**

`IsValid` immediately calls `value.Length` without guarding against `null`. If `TryParse` on any ID type is called with `null`, the method throws `NullReferenceException` rather than returning `false`. This violates the semantic contract of a `TryParse` pattern.

Fix:

```csharp
internal static bool IsValid(string value, string prefix)
{
    if (value is null) { return false; }
    // ... rest unchanged
```

---

### `Trakmark.Domain/Ids/UserAccountId.cs`

**Line 13 — Missing null guard [Medium]**

The public constructor `UserAccountId(string value)` accepts a reference type but does not call `ArgumentNullException.ThrowIfNull(value)`. Per CLAUDE.md this is required on all public constructors.

Note: this is a `readonly record struct` so the CLAUDE.md rule "do not use record for types with validation logic" also applies. `UserAccountId` performs no validation (it is a passthrough), so `readonly record struct` is acceptable here, but the null guard is still required.

---

### `Trakmark.Domain/Catalog/Event.cs`

**Lines 24-28 — Missing null guards [Medium]**

`discipline` and `sport` are reference types; neither is guarded. Fix:

```csharp
public Event(Discipline discipline, Sport sport)
{
    ArgumentNullException.ThrowIfNull(discipline);
    ArgumentNullException.ThrowIfNull(sport);
    Discipline = discipline;
    Sport = sport;
}
```

**Missing `==`/`!=` operator overloads [Low]**

`Event` implements `IEquatable<Event>` and overrides `Equals`/`GetHashCode` but does not expose `==`/`!=` operators. All other `IEquatable` types in the project that are not closed-set singletons (e.g., `PersonName`, `SchoolName`) consistently provide these operators. `Event` should as well.

---

### `Trakmark.Domain/Catalog/Discipline.cs`

**Lines 49, 58, 66, 73 — Missing null guards on factory methods [Medium]**

Four public factory methods accept `string` or reference-type parameters without guarding:

- `HurdleRun(int, HurdleHeight height)` — `height` is a reference type
- `ImplementThrow(string eventName, ImplementWeight weight)` — both are reference types
- `Jump(string eventName)` — `string` reference type
- `PlaceOnly(string eventName)` — `string` reference type

A null `eventName` would silently create a discipline with identity key `"jump:"` or `"placeonly:"`, which could cause silent catalog collisions.

**Missing `==`/`!=` operator overloads [Low]**

`Discipline` implements `IEquatable<Discipline>` and is used as a filter key in `BestMarksService` and `SeasonViewService`, but lacks `==`/`!=`. While the service code uses `.Equals(discipline)` explicitly, operator overloads prevent accidental reference comparisons at call sites.

---

### ~~`Trakmark.Domain/Catalog/Performance.cs`~~

~~**`Performance` base class should be `sealed` or documented as intentionally open [Medium]**~~

~~CLAUDE.md states "types are sealed by default; unseal only when inheritance is intended and designed for." `Performance` is abstract and unsealed by design (for `TimeMark`/`DistanceMark`), which is correct. However, the subtypes `TimeMark` and `DistanceMark` are `sealed` and implement `IEquatable<T>` correctly. Both are missing `==`/`!=` operator overloads.~~ ✓ Resolved

~~`TimeMark` and `DistanceMark` should expose:~~
```csharp
public static bool operator ==(TimeMark? left, TimeMark? right) => left?.Equals(right) ?? right is null;
public static bool operator !=(TimeMark? left, TimeMark? right) => !(left == right);
```

---

### `Trakmark.Domain/Catalog/Placement.cs`

**Missing `==`/`!=` operator overloads [Low]**

`Placement` implements `IEquatable<Placement>` without operator overloads. Consistent with all other `IEquatable` types in the project.

---

### `Trakmark.Domain/ValueObjects/GradeLevel.cs` (and `MarkKind`, `CompetitionLevel`, `Sport`)

**Lines 36 and 42 — `Equals`/`GetHashCode` comparer mismatch [Medium]**

CLAUDE.md explicitly states: "When `Equals` uses a specific `StringComparison`, `GetHashCode` must use the same comparer. Mismatched comparers silently break dictionary lookups."

The pattern used in `GradeLevel`, `MarkKind`, `CompetitionLevel`, and `Sport` is:
```csharp
public bool Equals(GradeLevel? other) => ReferenceEquals(this, other);   // identity equality
public override int GetHashCode() => Name.GetHashCode(StringComparison.Ordinal);  // value hash
```

`ReferenceEquals` is identity-based; `GetHashCode` is value-based. For two references to the same singleton `GradeLevel.Freshman`, `GetHashCode` will return the same value, so ordinary usage is not broken. But the contract is violated: two `Equals`-equal objects must produce the same hash. With `ReferenceEquals`, only the same object instance is considered equal, so `GetHashCode` must also return the object's identity hash, not a name-based hash.

Fix option A (identity hash — recommended for closed-set singletons):
```csharp
public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
```

Fix option B (name-based — removes the singleton enforcement from equality):
```csharp
public bool Equals(GradeLevel? other) =>
    other is not null && string.Equals(Name, other.Name, StringComparison.Ordinal);
public override int GetHashCode() => Name.GetHashCode(StringComparison.Ordinal);
```

Option A is preferred because the design intent is identity equality for singleton instances.

Note: `School._teams` is keyed by `Sport`. Under the current mismatch, `Dictionary<Sport, Team>` lookups work correctly today only because every `Sport` reference in normal use points to one of the two static singletons (`TrackAndField`, `CrossCountry`). If `Sport` were ever deserialized from persistence, a new instance with the same `Name` would produce a different identity hash, causing a silent dictionary miss.

Same fix applies to: `MarkKind`, `CompetitionLevel`, `Sport`, `Tier`, `HurdleHeight`, `ImplementWeight`.

---

### `Trakmark.Domain/Aggregates/Student.cs`

**Lines 28 and 39 — Missing null guards [Medium]**

`Student(StudentId id, PersonName name, ...)`: `PersonName` is a reference type; `name` should be null-guarded.
`AddEnrollment(SchoolId, SchoolYear, GradeLevel gradeLevel)`: `GradeLevel` is a reference type; `gradeLevel` should be null-guarded.

Note: `SchoolYear` is a `readonly record struct` (value type) — no guard needed there.

---

### `Trakmark.Domain/Aggregates/Enrollment.cs`

**Line 23 — Missing null guard on `gradeLevel` [Medium]**

`Enrollment` constructor accepts `GradeLevel gradeLevel` (a reference type) without guarding. This is a public constructor.

---

### `Trakmark.Domain/Aggregates/RegisteredUser.cs`

**Line 52 — Missing null guard on `AddStudent` [Medium]**

`PersonName` is a reference type; `name` is not guarded. `PersonName`'s own constructor will throw `ArgumentNullException` if `null` reaches it, but the guard should be at the first public boundary.

---

### `Trakmark.Domain/Aggregates/School.cs`

**Line 41 — Missing null guards on `Create` [Medium]**

`SchoolName name` and `CompetitionLevel level` are both reference types; neither is null-guarded in the `Create` factory. `AddTeam` (line 53) correctly guards `sport` — the inconsistency inside the same class is notable.

---

### `Trakmark.Domain/Aggregates/Meet.cs`

**Lines 53 and 79 — Missing null guards [Medium]**

`Meet.Create(MeetName name, MeetDate date, CompetitionLevel level, Sport sport)` — `MeetName`, `CompetitionLevel`, and `Sport` are reference types. None are null-guarded.

`Meet.RecordResult(..., Event @event, ...)` — `@event` is a reference type. A null `@event` causes `NullReferenceException` inside `EnforceSportMatch` at the `.Sport` access rather than a diagnostic `ArgumentNullException`.

---

### `Trakmark.Domain/Aggregates/Career.cs`

**Line 57 — O(n) linear scan before O(log n) binary insert [Medium]**

```csharp
if (_enrollments.Any(e => e.SchoolYear == enrollment.SchoolYear))
{
    return false;
}
var index = _enrollments.BinarySearch(enrollment, BySchoolYear);
```

`BinarySearch` returns a negative bitwise complement of the insertion point when no match is found, and a non-negative index when a match exists. The `Any` scan is redundant:

```csharp
internal bool TryAdd(Enrollment enrollment)
{
    var index = _enrollments.BinarySearch(enrollment, BySchoolYear);
    if (index >= 0)
    {
        return false;  // duplicate school year
    }
    _enrollments.Insert(~index, enrollment);
    return true;
}
```

This eliminates the O(n) scan and makes the whole operation O(log n).

**Note on `BinarySearch` comparer:** `BySchoolYear` compares by `SchoolYear` only. The `Enrollment.Equals` used for equality also includes `SchoolId` and `GradeLevel`. The `BinarySearch` call will only find a matching entry when `SchoolYear` matches; because the invariant is "one enrollment per school year" this is the correct key. The fix above is safe.

~~**Lines 41-48 — `AddEnrollment` duplicates `TryAdd` logic [Low]**~~

~~`AddEnrollment` calls `TryAdd` and then throws if it fails. This is the only public add path; `TryAdd` is internal. If `AddEnrollment` is only called from `Student.AddEnrollment`, and `Student.AddEnrollment` already uses `TryAdd` directly, the public `Career.AddEnrollment` is dead code that duplicates logic. Consider removing it or making `TryAdd` the single code path.~~ ✓ Resolved

---

### `Trakmark.Domain/Services/SeasonViewService.cs`

**Line 28 — Returns deferred `IEnumerable<Result>` [Low]**

`GetSeasonResults` returns `IEnumerable<Result>` from a LINQ `Where(...).OrderBy(...)` chain. The enumeration is deferred: callers that enumerate twice will perform the filtering twice. Consider returning `IReadOnlyList<Result>` with `.ToList()` to materialize results at the service boundary, making the contract explicit.

---

### `Trakmark.Domain/Services/StudentVisibilityService.cs`

**Line 39 — Nullable comparison on `UserAccountId?` [Low]**

```csharp
var isLinked = student.UserAccountId == user.AccountId;
```

`student.UserAccountId` is `UserAccountId?` (nullable value type). `user.AccountId` is `UserAccountId` (non-nullable). The `==` comparison for `readonly record struct` is auto-generated and will compare `.Value` strings using the default `EqualityComparer<string>`. When `student.UserAccountId` is `null` (normal pilot-era case), the nullable `==` returns `false`. This is functionally correct but relies on implicit nullable lifting semantics; an explicit null check would make the intent clearer:

```csharp
var isLinked = student.UserAccountId.HasValue
    && student.UserAccountId.Value == user.AccountId;
```

---

### `Trakmark.Domain/Aggregates/Result.cs`

~~**Lines 60-78 — Internal constructor reference parameters not null-guarded [Info]**~~

~~The `internal` constructor accepts `Event @event` and `Tier tier` as reference types without guards. `internal` constructors are exempt from the CLAUDE.md rule (which covers "public constructors and methods"), but `Meet` is the only caller and it already validates `@event` via `EnforceSportMatch`. No action required; noted for completeness.~~ ✓ Resolved

---

### `Trakmark.Domain.Tests/`

Tests use xUnit `[Fact]` and `[Theory]` correctly; `// Arrange / Act / Assert` comments are present. 160 tests covering 44 spec scenarios is strong coverage for a domain-only project. No bUnit needed (no Blazor components). NSubstitute is referenced but not visibly used — the domain layer has no interfaces to mock, so this is understandable; the reference is forward-looking.

One gap: no tests specifically exercise the concurrent ID generation path to validate thread safety (nor the null-input `TryParse` behavior). These would be valuable regression tests once the fixes are applied.

---

## Checklist

### Functionality
- [x] Core functionality works as intended (160 tests, 44 spec scenarios)
- [x] Edge cases handled (duplicate enrollment, sport mismatch, mark-kind mismatch)
- [x] ~~`DomainId.IsValid(null, prefix)` throws NRE instead of returning `false`~~
- [x] Configuration correct (no secrets in appsettings; no appsettings changes)

### Thread Safety
- [x] ~~`CrockfordBase32.Random` is not thread-safe — concurrent `NewId()` calls can corrupt RNG state~~
- [x] No other shared mutable state identified
- [x] Domain services are stateless

### Performance
- [ ] `Career.TryAdd` performs O(n) linear scan redundantly before O(log n) binary insert
- [x] No blocking I/O
- [x] `BestMarksService` and `SeasonViewService` are pure LINQ projections

### Code Quality
- [x] Follows existing patterns (sealed, XML docs, closed-set types)
- [x] Source-generated logging not applicable (domain layer, no ILogger calls — correct)
- [x] ~~Null guards missing on the majority of public constructors/methods~~
- [x] XML `<summary>` docs present on all public/internal members
- [x] Cyclomatic complexity within limit (largest method `EnforceStatusInvariant` is well-factored)
- [x] ~~`Equals`/`GetHashCode` comparer mismatch on all closed-set types~~

### Simplification and Refactoring
- [ ] `Career.TryAdd` O(n) duplicate-check is redundant with binary search
- [x] ~~`Career.AddEnrollment` duplicates `TryAdd` logic; consider removing if it is dead code~~
- [ ] `SeasonViewService.GetSeasonResults` should materialize to `IReadOnlyList<Result>`

### Blazor / Component Quality
- N/A — this PR introduces `Trakmark.Domain` only; no Blazor components are added

### Testing
- [x] Adequate coverage for all 44 spec scenarios
- [x] Follows project patterns (xUnit, `[Theory]`/`[InlineData]`, AAA comments)
- [ ] No test for `TryParse(null, out _)` returning `false` (would catch the `DomainId.IsValid` null bug)
- [ ] No concurrent ID generation test (would catch the `Random` thread-safety bug)

---

## Questions for Author

1. ~~**`Career.AddEnrollment` vs `TryAdd`:** `Student.AddEnrollment` uses `Career.TryAdd` directly. Is `Career.AddEnrollment` (the throwing overload) used anywhere, or is it dead code that can be removed?~~ ✓ Resolved

2. ~~**Closed-set type equality intent:** Was `ReferenceEquals` chosen deliberately to enforce singleton identity (i.e., callers cannot create "equal but different" instances by accident), or was name-based equality the intent? If singleton identity, `GetHashCode` should use `RuntimeHelpers.GetHashCode(this)` to match.~~ — Answer: name-based equality; two instances with the same Name are equal regardless of object reference.

3. ~~**`SeasonViewService` return type:** Was returning deferred `IEnumerable` intentional for lazy evaluation, or should the service materialize results? If callers compose further LINQ on top, deferred is fine. If callers enumerate once and store, materialization at the service boundary is safer.~~ — Deferred until persistence layer is implemented.

4. ~~**`UserAccountId` null guard:** Since `UserAccountId` is a bridge to ASP.NET Identity, is it ever valid for `value` to be `null` at construction time (e.g., as an empty placeholder)? If not, the guard should be added; if yes, the intent should be documented.~~ — Answer: add the guard. Already applied in b7a0226.

---

## References

- CLAUDE.md — project coding conventions
- [C# `Random.Shared` documentation](https://learn.microsoft.com/en-us/dotnet/api/system.random.shared)
- [MSDN — `RuntimeHelpers.GetHashCode`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.runtimehelpers.gethashcode)
- [`List<T>.BinarySearch` documentation](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.binarysearch)
- PR #3: https://github.com/farooq-teqniqly/trakmark/pull/3
