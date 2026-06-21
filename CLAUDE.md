# CLAUDE.md — dotnet-otel

## Language and spelling

Use U.S. English in all prose, comments, commit messages, and docs.

## Project structure and naming

- One project per layer/concern; name with the `Trakmark.<Area>` prefix (e.g. `Trakmark.Domain`). The web app project is `Trakmark`.
- Each project's unit tests live in a sibling project named `<Project>.Tests` (e.g. `Trakmark.Domain.Tests`).
- Test projects use **xUnit** and reference the project under test.
- Use **NSubstitute** for mocking in tests.
- One test class per production type, one file per test class — never group multiple unrelated types in a single test file.
- Each test has `// Arrange`, `// Act`, `// Assert` comments.
- Prefer data-driven tests (`[Theory]`/`[InlineData]`/`[MemberData]`) over many near-duplicate `[Fact]`s. Once a `[Theory]` covers a case, do not also write a `[Fact]` for the same scenario — it is redundant and will diverge.
- Test behavior, not implementation — assert observable outcomes, not internal calls, so tests aren't brittle.
- For integration and end-to-end tests, prefer the real database via **Testcontainers** over in-memory fakes.
- Do not test `internal` helpers directly. Cover them through their public API callers (e.g. test `DomainId.IsValid` by calling `TryParse`, not by invoking `IsValid` directly). If the helper is not reachable through any public surface, that is a design signal, not a reason to add a direct test.
- Practice **TDD**: when a spec defines behavior (e.g. OpenSpec `#### Scenario:` blocks), write the failing tests from those scenarios first, then implement to green. Each scenario maps to a test case. For test-only changes (adding tests against already-complete production code), satisfy the failing-first requirement by writing the test body with `Assert.Fail("not implemented")` as a placeholder, confirming the test fails, then replacing the placeholder with real assertions.
- Register every new project in `Trakmark.slnx`.

## Code conventions

- One type per file — never define multiple top-level types in the same `.cs` file. The file name must match the type name.
- Target **net10.0**; use latest C# language features where they improve clarity.
- Types are **`sealed` by default**; unseal only when inheritance is intended and designed for.
- Use `sealed record` (or `readonly record struct`) only when the type is a pure data carrier with no validation logic and no custom equality semantics. Domain value objects that enforce invariants in their constructor must be `sealed class`, not `record`, and must implement `IEquatable<T>` with a matching `Equals`/`GetHashCode` override **and** `==`/`!=` operator overloads. Exception: a `readonly record struct` is acceptable when (a) it wraps a single primitive value, (b) the only invariant is a range/null check on that value, and (c) the auto-generated structural equality is semantically correct for the domain. **Every type that implements `IEquatable<T>` must expose `==`/`!=` operator overloads — no exceptions.**
- When writing null-left operator assertions (`Assert.False(null == x)`), skip `readonly record struct` types — the compiler does not allow a typed null operand for a value type, so the null-guard branch is unreachable. Only reference-type value objects (`sealed class`) need null-left coverage.
- `Nullable` and `ImplicitUsings` enabled on all projects.
- No inline XML comments on code that is self-explanatory. Add `<summary>` XML docs on all `internal` and `public` types and members (constructors, methods, properties, non-trivial fields, and `const`s). Exceptions: `[LoggerMessage]` methods in logging classes (`*.Logging.cs`) — the message template is self-documenting; EF Core migration files (`Migrations/`) — auto-generated, do not edit; test methods (`[Fact]`, `[Theory]`) — the method name is the specification, an XML summary would restate it.
- Use **source-generated logging** (`[LoggerMessage]`) for all `ILogger` calls — never `LogInformation(...)` directly (CA1873).
- Split large partial classes by concern: e.g. `Foo.cs` for logic, `Foo.Logging.cs` for `[LoggerMessage]` declarations.
- Keep cyclomatic complexity of any method at **15 or below**; extract helpers when a method would exceed this.
- Validate all public constructor and method parameters that accept reference types: use `ArgumentNullException.ThrowIfNull(param)` as the first line. Exception: DI-injected dependencies (trust the container). **After writing every new public/internal type, scan each public constructor and method — confirm every reference-type parameter has the guard before moving on.**
- When `Equals` uses a specific `StringComparison`, `GetHashCode` must use the same comparer (e.g. `Value.GetHashCode(StringComparison.Ordinal)`). Mismatched comparers silently break dictionary lookups.
- No defensive null-checks on DI-injected dependencies — trust the container.
- Remove any DI-injected dependency that is not used in the file it is injected into.
- Use `null!` (not `default!`) to suppress nullable warnings on uninitialized required properties.
- Always use braces for control statements (`if`, `else`, `for`, `foreach`, `while`, `do`) — even single-line bodies.
- No comments that restate what the code already says.

## Configuration

- Secrets (API keys) and **connection strings** go in **user secrets**, never in `appsettings.json`.
- Docker Compose **passwords** go in a **`.env` file**, never hardcoded in `docker-compose.yml`.
- Endpoint overrides go in `appsettings.json` with sensible defaults in code.
- `Properties/launchSettings.json` sets `DOTNET_ENVIRONMENT=Development`.

## Git commits

- Subject line: ≤ 50 characters, imperative mood, no period.
- Body (optional): wrap at 72 characters, explain _why_ not _what_.
- Total commit message: **under 50 words**.
- Always add `Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>` when AI-assisted.

## Pull request descriptions

- **Under 200 words.**
- Include: what changed, why, and a short test/verification note.
- No filler phrases ("this PR...", "in this change...").

## Pre-merge checklist

Before merging any branch, resolve all SonarQube warnings using a clean build:

```powershell
dotnet clean .\Trakmark\Trakmark.slnx
dotnet build .\Trakmark\Trakmark.slnx 2>&1 |
    Select-String "warning S\d+" |
    Where-Object { $_.Line -notmatch "Microsoft\.Common" } |
    ForEach-Object { $_.Line.Trim() }
```

- Attempt to fix or suppress each warning. Repeat for up to **3 rounds**.
- If warnings remain after 3 rounds, **block the merge** and write the outstanding items to `docs/sonarqube-warnings-triage.md` (date-stamped entry, branch name, remaining warning list, reason each could not be resolved).

`Trakmark.Domain` line coverage must be **100%** before merging. Run the `coverage-report` skill (or its `Run-Coverage.ps1`) and add tests to close any gap — domain types have no untestable infrastructure dependencies, so an uncovered line means a missing test, not an exemption.
