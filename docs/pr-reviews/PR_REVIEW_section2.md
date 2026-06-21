# PR Review — Section 2: Domain: CityId

Branch: `worktree-agent-a5d0d34d8ee46481a` (scoped against `add-cities`)
Scope: `git diff add-cities..HEAD` (commit `ec89614` — "Add CityId strongly-typed identifier")

Reviewed commit: ec89614 on branch worktree-agent-a5d0d34d8ee46481a

No GitHub PR exists for this change yet, so `gh api` comment fetch was skipped per instructions.

## Summary of findings

| Severity | Tag | Topic | Location | Source |
|---|---|---|---|---|
| Low | [New] | `Parse`/`TryParse` accept `string` without `ArgumentNullException.ThrowIfNull` on `Parse`, relying on `FormatException` instead of conventional `ArgumentNullException` for null input | `Trakmark.Domain/Ids/CityId.cs:21-24` | Claude |
| Info | [New] | Pattern fidelity confirmed — `CityId` is a byte-for-byte mirror of the merged `SchoolId` precedent | `Trakmark.Domain/Ids/CityId.cs`, `Trakmark.Domain/Ids/SchoolId.cs` | Claude |
| Info | [New] | `CityIdTests` omits `NewId_GeneratesUniqueValues`-style structural redundancy issue — none found; test file follows `SchoolIdTests` shape closely and adds the uniqueness `[Fact]` `SchoolIdTests` lacks | `Trakmark.Domain.Tests/Ids/CityIdTests.cs:39-48` | Claude |

## Existing review comments

None — no PR exists yet, so there are no `gh api` review/issue comments to index.

## TL;DR

`CityId.cs` and `CityIdTests.cs` are a clean, faithful port of the already-merged `SchoolId` pattern (prefix `"CTY-"`, backed by the shared `DomainId`/`CrockfordBase32` helpers). The implementation is correct, the `readonly record struct` choice is justified under the CLAUDE.md exception clause (wraps a single primitive `Value`, only a format/length invariant, auto-generated structural equality is correct), and all tests pass review by inspection. The one notable deviation — `Parse`/`TryParse` not guarding null with `ArgumentNullException.ThrowIfNull` — is inherited unchanged from `SchoolId` and is not a regression introduced by this PR, but it is worth flagging since it's a real CLAUDE.md gap that will now exist in two places instead of one. `tasks.md` checkboxes 2.1–2.3 are accurately marked complete; sections 3+ remain untouched as expected.

## Architecture / data flow

`CityId` sits in `Trakmark.Domain/Ids/` alongside `SchoolId`, both backed by the internal `DomainId` helper (validates `PREFIX-BODY` shape, `BodyLength = 6`) and `CrockfordBase32` (31-character alphabet excluding ambiguous `0/O/1/I/L`). No other production code references `CityId` yet — it is purely additive, matching `tasks.md` section 2 scope. Section 3 (the `City` value object that will consume `CityId`) is correctly left unimplemented.

## File-by-file notes

### `Trakmark.Domain/Ids/CityId.cs`

Structurally identical to `Trakmark.Domain/Ids/SchoolId.cs` with only the prefix (`"CTY-"` vs `"SCH-"`) and type name swapped:

- `readonly record struct CityId` — correct per the CLAUDE.md exception (single primitive `Value`, format-only invariant, structural equality is semantically correct for an opaque ID).
- Private constructor + static factories (`NewId`, `Parse`, `TryParse`, `Empty`) — matches `SchoolId` exactly.
- XML `<summary>` present on the type and all public members. `<inheritdoc/>` correctly used for `ToString()`.
- `Parse(string value)` and `TryParse(string value, out CityId id)` (`CityId.cs:21,30`) take a reference-type `string` parameter without `ArgumentNullException.ThrowIfNull`. Per CLAUDE.md: "Validate all public constructor and method parameters that accept reference types... `ArgumentNullException.ThrowIfNull(param)` as the first line." Currently a null `value` flows into `DomainId.IsValid`, which explicitly null-checks and returns `false` — so `TryParse(null, out _)` returns `false` (reasonable, matches `int.TryParse`-style conventions) but `Parse(null)` throws `FormatException` rather than the more conventional `ArgumentNullException` that .NET's own `Parse` family throws for null input (e.g. `Guid.Parse(null)` → `ArgumentNullException`). This is pre-existing in `SchoolId` (not introduced by this PR) but now duplicated. Recommend, in a follow-up, adding `ArgumentNullException.ThrowIfNull(value)` at the top of `Parse` only (leave `TryParse` as-is, since `TryParse` conventionally treats null as a failed parse, not an exception) — this should be done for both `SchoolId` and `CityId` together to keep the two types consistent.
- `==`/`!=` operators: not explicitly declared, but correctly not required — `readonly record struct` auto-generates value equality and `==`/`!=`; CLAUDE.md's "every `IEquatable<T>` type must expose operators" clause is satisfied implicitly by the record struct's compiler-generated members, consistent with the `SchoolId` precedent.
- No `ArgumentNullException` needed on `NewId()` (no parameters) or the private constructor (internal use only, never receives untrusted input).
- Cyclomatic complexity well under 15 for every member.

### `Trakmark.Domain.Tests/Ids/CityIdTests.cs`

- One test class per production type, one file per class — compliant.
- `[Theory]`/`[InlineData]` used for the malformed-input cases; no redundant `[Fact]` duplicates a `[Theory]`-covered case.
- `// Arrange`, `// Act`, `// Assert` comments present on every test (collapsed to `// Arrange / Act / Assert` where the assertion is the actual invocation — consistent with `SchoolIdTests.cs`).
- `CityId_TryParse_ReturnsFalseForMalformedInput` (`CityIdTests.cs:8-17`) exercises wrong-prefix, too-short, too-long, and invalid-character (`"O"`, excluded from the Crockford alphabet) cases — covers `DomainId.IsValid` only indirectly through the public `TryParse` API, per CLAUDE.md's "do not test internal helpers directly" rule.
- `CityId_Parse_ThrowsFormatExceptionForMalformedInput` (`CityIdTests.cs:19-27`) covers wrong-length, wrong-format, and empty-body cases.
- `CityId_RoundTrips` (`CityIdTests.cs:29-37`) and `CityId_NewId_GeneratesUniqueValues` (`CityIdTests.cs:39-48`) are `[Fact]`s — each covers a single non-data-driven scenario (round-trip identity, uniqueness across two calls), so a `[Theory]` would add no value here; correctly left as `[Fact]`.
- Note: `CityIdTests` adds a `NewId_GeneratesUniqueValues` test that `SchoolIdTests` (the precedent file) does not have — a positive divergence, not a problem; the tasks.md instruction explicitly calls for "NewId uniqueness" coverage, and this satisfies it. Consider back-porting the same test to `SchoolIdTests.cs` in a future cleanup for parity, but that is out of scope for this PR.
- No mocking required (pure value type, no dependencies) — NSubstitute is correctly not used.

### `openspec/changes/add-cities/tasks.md`

Checkboxes 2.1, 2.2, 2.3 are marked complete and accurately reflect the work done (failing tests written first per TDD, implementation following the `SchoolId` pattern, tests passing). Section 3+ remain unchecked, correctly out of scope for this PR.

## Checklist

### Functionality
- [x] Core functionality works as intended (Parse/TryParse/NewId/Empty all correct by inspection and mirror a working precedent)
- [x] Edge cases handled (too-short, too-long, wrong-prefix, invalid-character, null body via `DomainId.IsValid`)
- [x] Error scenarios covered (`FormatException` on `Parse` failure)
- [x] Configuration correct (no configuration surface for this type)

### Thread safety
- [x] Concurrent operations safe (immutable `readonly record struct`, `Random.Shared` is thread-safe)
- [x] Synchronization appropriate (none needed)
- [x] No race conditions
- [x] Disposal thread-safe (no disposable resources)

### Performance
- [x] No unnecessary blocking
- [x] Efficient algorithms/data structures (O(prefix+body) string ops)
- [x] Proper caching strategies (N/A)
- [x] Background ops don't impact request paths (N/A)

### Code quality
- [x] Follows existing patterns (exact mirror of `SchoolId`)
- [x] Proper error handling and logging (no `ILogger` usage in this type; N/A)
- [x] Clear and maintainable
- [x] Appropriate separation of concerns (`DomainId`/`CrockfordBase32` reused, not duplicated)
- [x] Human reviewer rename requests implemented (none requested)
- [x] XML `<summary>` docs present on all public/internal members
- [x] Cyclomatic complexity ≤ 15
- [ ] `ArgumentNullException.ThrowIfNull` on all public ref-type parameters — gap on `Parse(string value)` (see finding above); pre-existing in `SchoolId`, now duplicated

### Simplification & refactoring
- [x] No duplicated logic across sync/async variants (N/A, no async)
- [x] Repeated error-mapping/catch/boilerplate extracted to helpers (`DomainId`/`CrockfordBase32` shared)
- [x] Large methods decomposed into single-responsibility pieces (all methods trivially small)
- [x] No copy-pasted blocks consolidatable (DRY) — `CityId`/`SchoolId` duplication is intentional per the established strongly-typed-ID pattern, not a DRY violation

### Blazor / component quality
- N/A — this PR contains no Blazor/UI code.

### Testing
- [x] Adequate coverage (NewId uniqueness, round-trip, malformed Parse/TryParse all covered)
- [x] Follows project patterns (xUnit, one class per type, Arrange/Act/Assert)
- [x] Error cases tested (`FormatException`, `TryParse` false path)
- [x] Integration tests if applicable (N/A — pure value type)
- [x] Blazor component tests use bUnit where applicable (N/A)

## Questions for author

1. Should the pre-existing `Parse`/`ArgumentNullException` gap in `SchoolId` be fixed now (and ported to `CityId` for consistency), or tracked as a follow-up once `City`/`School` consumers exist and the null-input contract matters in practice?
2. Do you want `SchoolIdTests.NewId_GeneratesUniqueValues` back-ported to match the new `CityIdTests` coverage, for parity between the two ID test suites?

## Process Improvements

No cross-cutting process gaps identified — the only finding (missing `ArgumentNullException.ThrowIfNull` on `Parse`) is a single pre-existing pattern carried forward from `SchoolId`, not a new pattern introduced by this PR, and does not yet recur across three or more independent findings.

## References

- `Trakmark.Domain/Ids/CityId.cs`
- `Trakmark.Domain/Ids/SchoolId.cs` (precedent)
- `Trakmark.Domain/Ids/DomainId.cs`
- `Trakmark.Domain/Ids/CrockfordBase32.cs`
- `Trakmark.Domain.Tests/Ids/CityIdTests.cs`
- `Trakmark.Domain.Tests/Ids/SchoolIdTests.cs` (precedent)
- `openspec/changes/add-cities/tasks.md`
- `CLAUDE.md`
