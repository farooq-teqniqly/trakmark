# Retro — add-cities §5 — 2026-06-25

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Developer called `City.Create` twice (validate pass + build pass), generating different CityIds each time; caught in round-1 review | Added rule: factory methods that generate a new identity must be called exactly once per entity construction | `CLAUDE.md` (Code conventions) |
| `dotnet ef migrations add` for the Cities unique index also captured unrelated Identity schema drift (AspNetUserPasskeys, column widths), requiring a split into two migrations in round-2 fixes | Added prerequisite: before adding any new migration, run `dotnet ef migrations add _check ...` to detect pending drift; if drift is unrelated, remove the check migration and scaffold a dedicated drift migration first | `CLAUDE.md` (Configuration) |
| `PersistAsync` had no `DbUpdateException` catch for the Cities unique index, so duplicate-city inserts surfaced as unhandled exceptions rather than domain results; caught in review | Added rule: any service that saves to a unique-indexed table must catch `DbUpdateException`, inspect inner `SqlException` error numbers 2601/2627, and return a domain-level duplicate result | `CLAUDE.md` (Code conventions) |
| `ApplicationDbContextFactory` hard-coded a LocalDB connection string in source, violating the "connection strings in user secrets" rule | Added rule documenting the approved exception: design-time factories must read from an env var with a LocalDB fallback; never a literal string in source | `CLAUDE.md` (Configuration) |
| Migration drift check and factory-once / DbUpdateException rules need to appear in the developer agent's self-review checklist to be caught before commit | Two new self-review checklist items + expanded migration prerequisite section drafted for `.claude/agents/developer.md` — **not applied: Write/Edit to `.claude/agents/` was denied in the retrospective agent context. Apply manually.** | `.claude/agents/developer.md` (pending manual) |

## Pending manual edits to `.claude/agents/developer.md`

### 1. Expand the "Persistence/migration prerequisite" section

Add after the existing `dotnet-ef` install paragraph:

```
Before running `dotnet ef migrations add <YourMigration> ...`, first run:

    dotnet ef migrations add _check --project <migration-project> --startup-project <web-project>

Open the generated `_check` migration file. If it contains changes unrelated
to the current section (e.g., Identity schema drift, column-width changes from
prior work), immediately run `dotnet ef migrations remove` and scaffold a
dedicated migration for that drift first, then proceed with your own migration.
Never let pre-existing drift ride in the same migration as intentional changes.
```

### 2. Add two items to the "Self-review checklist before committing" section

```
- **Factory method called once per entity**: if you have a validate-then-build
  pattern, do not call `Entity.Create(...)` in both the validate pass and the
  build pass — each call generates a fresh identity. Validate inputs first,
  call the factory exactly once, then use the returned instance.
- **DbUpdateException on unique-index writes**: every service method that saves
  to a table protected by a unique index must catch `DbUpdateException`, inspect
  the inner `SqlException` for error numbers 2601 and 2627, and return a
  domain-level duplicate result rather than propagating the exception to the
  caller.
```

## Carry-forward (not yet actionable)

- **Worktree settings seeding** (friction 1): the orchestrator already documents
  the patch-then-relaunch workaround in `trakmark-engineering-team.md`. A
  longer-term fix would be a project-level `.claude/settings.json` that
  pre-grants Read/Glob/Grep so fresh worktrees inherit them without patching.
  Blocked on Claude tooling support for inheritable worktree permissions.
- **Reviewer false-alarm on tasks.md** (friction 6): the reviewer flagged
  tasks 5.1–5.3 as unchecked when they were already checked. This is a reviewer
  accuracy issue, not a rule gap. Carry forward as a known limitation; no rule
  change addresses it.
