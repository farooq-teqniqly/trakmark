# PR Review — Section 3: Domain: City

Branch: `worktree-agent-a4905682645909aa6` (scoped against `add-cities`)
Scope: `git diff add-cities..HEAD` (commit `177bc39` — "Add City value object with name+state equality")

Reviewed commit: 106f01b on branch worktree-agent-a4905682645909aa6

No GitHub PR exists for this change yet, so `gh api` comment fetch was skipped per instructions.

## Changelog

- **Round 1** (`177bc39`): initial review of `City` value object — one Medium finding (missing trim-and-accept test).
- **Round 2** (`106f01b`): fix commit "Add test for City name trim behavior" adds `Create_NameWithSurroundingWhitespace_TrimsName`, closing the round-1 Medium finding. Scope: `git diff 177bc39..HEAD` (single-file, 14-line addition).

## Summary of findings

| Severity | Tag | Topic | Location | Source |
|---|---|---|---|---|
| ~~Medium~~ | ~~[Resolved in 106f01b]~~ | ~~Name-trimming behavior (`name.Trim()`, documented on `Name` and `Create`) has no test exercising a name with leading/trailing whitespace that still produces a valid, trimmed result~~ | ~~`Trakmark.Domain/ValueObjects/City.cs:41`, `Trakmark.Domain.Tests/ValueObjects/CityTests.cs`~~ | ~~Claude~~ |
| Medium | [New] | `Trakmark.Domain` package line coverage is 99.85% (not the required 100%) due to a pre-existing gap in `Trakmark.Domain.Aggregates.Meet` (98.79% line rate); not introduced by this PR but blocks merge per CLAUDE.md's 100%-domain-coverage gate | `Trakmark.Domain/Aggregates/Meet.cs` | Claude |
| Info | [New] | `City.cs`/`CityTests.cs` fully satisfy all six City-related OpenSpec scenarios (valid create, empty/whitespace name, over-length name, null state, equal-by-name+state, unequal on differing name/state) | `Trakmark.Domain/ValueObjects/City.cs`, `Trakmark.Domain.Tests/ValueObjects/CityTests.cs` | Claude |
| Info | [New] | Pattern fidelity confirmed against `State.cs` precedent — `sealed class City : IEquatable<City>`, matching `StringComparison.OrdinalIgnoreCase` in both `Equals` and `GetHashCode`, `==`/`!=` operators present | `Trakmark.Domain/ValueObjects/City.cs:62-82` | Claude |
| Info | [New] | `City.Create` cyclomatic complexity is well under the 15-method ceiling (single happy path + two guard branches) | `Trakmark.Domain/ValueObjects/City.cs:36-59` | Claude |
| Info | [New] (Round 2) | `Create_NameWithSurroundingWhitespace_TrimsName` correctly exercises the trim-and-accept path (`"  Springfield  "` → `"Springfield"`), follows `// Arrange`/`// Act`/`// Assert` convention, uses `const string name`, and is a single targeted `[Fact]` (not a redundant `[Theory]` duplicate of the existing whitespace-rejection theory) | `Trakmark.Domain.Tests/ValueObjects/CityTests.cs:62-75` | Claude |

## Existing review comments

None — no PR exists yet, so there are no `gh api` review/issue comments to index.

## TL;DR

`City.cs` and `CityTests.cs` are a clean, correct implementation of the `City` value object: `sealed class` (not `record`, correctly avoiding the record exception since `City` has constructor invariants), `IEquatable<City>` with matching `OrdinalIgnoreCase` comparers in `Equals`/`GetHashCode`, `==`/`!=` operators, `ArgumentNullException.ThrowIfNull` on both reference-type parameters (`name`, `state`), and a private constructor reachable only through the validating `Create` factory. The build is clean (0 warnings/errors) and all 358 tests in the solution pass, including the 17 in `CityTests.cs`. Every OpenSpec scenario for `City` creation and equality maps to a test. The one functional gap is that `Create` trims the name (and documents this on both the `Name` property and the `Create` XML doc) but no test ever passes a name with leading/trailing whitespace and asserts the trimmed result — `Create_NameAtMaximumLength_CreatesCity` and `Create_ValidNameAndState_CreatesCityWithNewIdNameAndState` both use names with no surrounding whitespace, so the trim is implemented but unverified. ~~Separately, the CLAUDE.md pre-merge gate requires 100% `Trakmark.Domain` line coverage; the measured rate is 99.85%, but the shortfall traces entirely to `Trakmark.Domain/Aggregates/Meet.cs` (98.79%), a file untouched by this branch (last modified in PR #12, well before `add-cities` diverged) — `City.cs` itself measures 100% line and branch coverage. This is a pre-existing condition that should be tracked separately, not treated as a regression in this section, but it will block merge of `add-cities` until resolved (in this section or a follow-up).~~ ✓ Resolved (see Round 2 below for current coverage status of the `Meet.cs` item, which remains open as a separate, out-of-scope concern)

### Round 2 TL;DR

The fix commit `106f01b` is a minimal, surgical addition: a single new `[Fact]` (`Create_NameWithSurroundingWhitespace_TrimsName`) inserted into the existing `CityTests.cs`, 14 lines total, no production code touched. It asserts `City.Create("  Springfield  ", State.Illinois).Name == "Springfield"`, which directly exercises the `Create` trim path (`City.cs:41`) that was previously implemented but unverified. The test follows every project convention already established in the file: `// Arrange`/`// Act`/`// Assert` comments, `const string name` for the literal, a single targeted `[Fact]` rather than expanding the unrelated whitespace-rejection `[Theory]` (which tests a different scenario — rejection, not trim-and-accept — so a separate `[Fact]` is correct, not redundant). `dotnet test` confirms 18 tests now pass in `CityTests.cs` (17 → 18) and 359 total in the solution test run (358 → 359), with zero failures and zero new warnings. No new findings are introduced by this commit. The round-1 Medium finding on missing trim coverage is fully resolved. The only remaining open item from round 1 — the `Trakmark.Domain` package-wide coverage gap caused by pre-existing `Meet.cs` — is untouched by this commit and remains a release-level concern, not a section-3 blocker, per the original round-1 disposition.

## Architecture / data flow

`City` sits in `Trakmark.Domain/ValueObjects/` alongside the already-merged `State`, and depends on `Trakmark.Domain/Ids/CityId` (merged in section 2) for its identity. `City.Create` is the sole construction path (private constructor), generates a new `CityId` via `CityId.NewId()`, validates and trims `name`, and stores the supplied `State` reference directly (no defensive copy needed — `State` is itself an immutable value object). No other production code references `City` yet (persistence, application-layer batch save, and the Blazor UI are explicitly out of scope per `tasks.md` sections 4–6, which remain unchecked). This is purely additive and matches the section 3 task boundary. Round 2 makes no architectural changes — it is test-only.

## File-by-file notes

### `Trakmark.Domain/ValueObjects/City.cs`

- `sealed class City : IEquatable<City>` — correct choice per CLAUDE.md: `City.Create` enforces invariants (non-empty, ≤100 chars, non-null state) in the constructor path, so the `record`/`readonly record struct` exception does not apply.
- `Equals`/`GetHashCode` both use `StringComparison.OrdinalIgnoreCase` for `Name` (`City.cs:64`, `City.cs:72`) — comparers match, satisfying the CLAUDE.md rule that mismatched comparers silently break dictionary lookups.
- `==`/`!=` operators present and correctly implemented with the `left?.Equals(right) ?? right is null` null-safe pattern (`City.cs:78-82`), matching the `State` precedent exactly.
- `ArgumentNullException.ThrowIfNull(name)` and `ArgumentNullException.ThrowIfNull(state)` are both present as the first two lines of `Create` (`City.cs:38-39`) — compliant with the CLAUDE.md reference-type-parameter guard rule.
- XML `<summary>` present on the type, all public members (`Id`, `Name`, `State`, `Create`, `Equals`, `GetHashCode`, `ToString`, `==`, `!=`), and the private constructor is appropriately left undocumented (not public/internal-facing per the CLAUDE.md exception scope, though note the constructor is `private` so the doc requirement does not technically apply — consistent with `State.cs`).
- `Create` (`City.cs:36-59`): cyclomatic complexity is low (2 guard branches + 1 happy path) — well within the 15-method ceiling.
- No changes in round 2 — this file is untouched by `106f01b`.

### ~~`Trakmark.Domain.Tests/ValueObjects/CityTests.cs` — Gap: trim-and-accept path untested~~

**[Resolved in 106f01b]** — `Create` trims `name` (`var trimmed = name.Trim();`, `City.cs:41`) and the `Name` property doc explicitly says "The trimmed name of this city" (`City.cs:16`), but no test supplied a name with leading/trailing whitespace around otherwise-valid content and asserted the trimmed result. The existing whitespace-only `[Theory]` (`""`, `" "`, `"   "`) only covered the rejection path, not the trim-and-accept path. Round 2's fix commit adds exactly the recommended test:
```csharp
[Fact]
public void Create_NameWithSurroundingWhitespace_TrimsName()
{
    // Arrange
    const string name = "  Springfield  ";
    var state = State.Illinois;

    // Act
    var city = City.Create(name, state);

    // Assert
    Assert.Equal("Springfield", city.Name);
}
```
Verified: builds clean, `dotnet test --filter "FullyQualifiedName~CityTests"` shows 18/18 passing (was 17), full solution run shows 359/359 passing (was 358), zero failures, zero new warnings.

### `Trakmark.Domain.Tests/ValueObjects/CityTests.cs` (general)

- One test class per production type, one file per class — compliant.
- `// Arrange` / `// Act` / `// Assert` comments present on every test (collapsed to `// Arrange / Act / Assert` where the call is the assertion itself, e.g. `Create_NullState_ThrowsArgumentNullException`) — consistent with project convention. Round 2's new test follows this exactly.
- `[Theory]`/`[InlineData]` correctly used for the empty/whitespace-name cases (`Create_EmptyOrWhitespaceName_ThrowsArgumentException`) and the equality matrix (`Equals_ComparesByNameCaseInsensitiveAndState`) — no redundant `[Fact]` duplicates a `[Theory]`-covered case. The new round-2 `[Fact]` is correctly kept separate from this theory since it tests acceptance-with-trim, not rejection.
- `Equals_Null_ReturnsFalse` (`CityTests.cs:102-114`, now shifted +14 lines) covers all four null-comparison directions (`city.Equals(null)`, `city == null`, `city != null`, `null == city`, `null != city`) — required since `City` is a `sealed class` (reference type), not exempted under the `readonly record struct` carve-out.
- `GetHashCode_MatchesForEqualNamesWithDifferentCase` directly verifies the `Equals`/`GetHashCode` comparer-consistency rule from CLAUDE.md.
- `StateFromAbbreviation` helper is a small, scoped lookup limited to the two states used in the equality theory (`IL`, `MO`) — reasonable given `State` exposes no `Parse`/`TryParse` by abbreviation; not over-engineered.
- Every OpenSpec scenario maps to a test:
  - "Create a valid city" → `Create_ValidNameAndState_CreatesCityWithNewIdNameAndState`
  - "City name must not be empty" → `Create_EmptyOrWhitespaceName_ThrowsArgumentException`
  - "City name must not exceed maximum length" → `Create_NameExceedsMaximumLength_ThrowsArgumentException` (plus the boundary `Create_NameAtMaximumLength_CreatesCity`)
  - "City state is required" → `Create_NullState_ThrowsArgumentNullException`
  - "Cities with same name and state are equal" / "different states" / "different names" → all three rows of `Equals_ComparesByNameCaseInsensitiveAndState`
  - Null-name guard (implementation detail beyond the spec's literal wording, but required by CLAUDE.md's parameter-guard rule) → `Create_NullName_ThrowsArgumentNullException`
  - Trim-and-accept (implementation detail, documented behavior) → `Create_NameWithSurroundingWhitespace_TrimsName` (added round 2)

### `openspec/changes/add-cities/tasks.md`

Checkboxes 3.1, 3.2, 3.3 are marked complete and accurately reflect the work done (failing tests written first per TDD, implementation following the documented `City` design, tests passing). Sections 4+ remain unchecked, correctly out of scope for this PR. No changes in round 2.

## Checklist

### Functionality
- [x] Core functionality works as intended
- [x] Edge cases handled (empty, whitespace, max-length boundary, null name, null state)
- [x] Error scenarios covered (`ArgumentException`, `ArgumentNullException`)
- [x] ~~Trim behavior verified by test (see Medium finding above)~~

### Thread safety
- [x] No mutable shared state; `City` instances are immutable after construction
- [x] N/A — no concurrency surface in this section

### Performance
- [x] No unnecessary allocations beyond what's required (single `Trim()` call)
- [x] N/A — no caching/background-op surface in this section

### Code quality
- [x] Follows existing `State.cs` pattern
- [x] `ArgumentNullException.ThrowIfNull` present on both reference-type parameters
- [x] `Equals`/`GetHashCode` use matching `StringComparison.OrdinalIgnoreCase`
- [x] `==`/`!=` operators present
- [x] XML `<summary>` docs present on all public members
- [x] Cyclomatic complexity well under 15
- [x] No comments restating code

### Simplification & refactoring
- [x] No duplicated logic across sync/async variants (N/A, no async)
- [x] No copy-pasted blocks needing consolidation

### Testing
- [x] xUnit, data-driven `[Theory]` used where appropriate, no redundant `[Fact]`/`[Theory]` overlap
- [x] `// Arrange`/`// Act`/`// Assert` comments on every test
- [x] Error cases tested
- [x] ~~Trim-and-accept path (whitespace-padded valid name) untested — see Medium finding~~
- [x] All 18 `CityTests` pass; all 359 solution tests pass (round 2: was 17/358)
- [x] `City.cs` measures 100% line/branch coverage in isolation
- [ ] `Trakmark.Domain` package-wide coverage is 99.85%, short of the CLAUDE.md 100% gate (gap is in pre-existing `Meet.cs`, not this section's code) — unchanged by round 2, remains open

## Questions for author

1. ~~Should the missing whitespace-trim test be added in this section, or is it acceptable to defer since the behavior is exercised indirectly through `Create_NameAtMaximumLength_CreatesCity`'s use of `Assert.Equal(maxLengthName, city.Name)` (which only proves no-op trimming on an already-clean string)?~~ **Resolved in 106f01b** — test added directly.
2. The `Trakmark.Domain` 100%-coverage gate currently fails due to `Meet.cs`, a file unrelated to `add-cities`. Is closing that gap in scope for this branch (e.g. as a follow-up commit before the section 3 → main merge), or should it be tracked as a separate maintenance item so `add-cities` isn't blocked on unrelated legacy debt? (Still open as of round 2; `106f01b` did not touch `Meet.cs`.)

## Process improvements

No new cross-cutting pattern emerged in this section — both findings are isolated (one missing test case, now resolved; one pre-existing coverage gap in unrelated code). The CLAUDE.md 100%-domain-coverage gate already exists as a process control; the `Meet.cs` gap is exactly the kind of finding it's designed to catch, so no additional rule is needed beyond ensuring the pre-merge coverage check is actually run (and blocking) before each section's "done" gate, not just at the final `add-cities` merge.

## References

- `openspec/changes/add-cities/specs/manage-cities/spec.md` — City creation and equality scenarios (lines 14-46)
- `openspec/changes/add-cities/tasks.md` — section 3 task definitions (lines 13-17)
- `Trakmark.Domain/ValueObjects/State.cs` — precedent pattern for value-object equality
- `docs/PR_REVIEW_section2.md` — prior section review (CityId), format precedent for this document
