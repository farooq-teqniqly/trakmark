# CLAUDE.md — dotnet-otel

## Language and spelling

Use U.S. English in all prose, comments, commit messages, and docs.

## Project structure and naming

- One project per layer/concern; name with the `Trakmark.<Area>` prefix (e.g. `Trakmark.Domain`). The web app project is `Trakmark`.
- Each project's unit tests live in a sibling project named `<Project>.Tests` (e.g. `Trakmark.Domain.Tests`).
- Test projects use **xUnit** and reference the project under test.
- Use **NSubstitute** for mocking in tests.
- Each test has `// Arrange`, `// Act`, `// Assert` comments.
- Prefer data-driven tests (`[Theory]`/`[InlineData]`/`[MemberData]`) over many near-duplicate `[Fact]`s.
- Test behavior, not implementation — assert observable outcomes, not internal calls, so tests aren't brittle.
- For integration and end-to-end tests, prefer the real database via **Testcontainers** over in-memory fakes.
- Practice **TDD**: when a spec defines behavior (e.g. OpenSpec `#### Scenario:` blocks), write the failing tests from those scenarios first, then implement to green. Each scenario maps to a test case.
- Register every new project in `Trakmark.slnx`.

## Code conventions

- Target **net10.0**; use latest C# language features where they improve clarity.
- Types are **`sealed` by default**; unseal only when inheritance is intended and designed for.
- `Nullable` and `ImplicitUsings` enabled on all projects.
- No inline XML comments on code that is self-explanatory. Add `<summary>` XML docs on all `internal` and `public` types and members (constructors, methods, properties, and non-trivial fields). Exceptions: `[LoggerMessage]` methods in logging classes (`*.Logging.cs`) — the message template is self-documenting; EF Core migration files (`Migrations/`) — auto-generated, do not edit.
- Use **source-generated logging** (`[LoggerMessage]`) for all `ILogger` calls — never `LogInformation(...)` directly (CA1873).
- Split large partial classes by concern: e.g. `Foo.cs` for logic, `Foo.Logging.cs` for `[LoggerMessage]` declarations.
- Keep cyclomatic complexity of any method at **15 or below**; extract helpers when a method would exceed this.
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
