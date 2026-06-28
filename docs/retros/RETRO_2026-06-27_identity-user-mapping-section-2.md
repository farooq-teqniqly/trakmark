# Retro — identity-user-mapping section 2 (Audit infrastructure) — 2026-06-27

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Commit `4d4f5c8 fix: remove redundant summary before inheritdoc` — developer agent placed both `<summary>` and `<inheritdoc/>` on the `SavingChanges` override; the re-reviewer caught this as a Low finding. Root cause: the XML-doc rule said "add `<summary>` on all public members" with no carve-out for overrides. | Added rule: for overrides of base-class or interface members, `/// <inheritdoc/>` alone satisfies the `<summary>` requirement — do not place an explicit `<summary>` block alongside `<inheritdoc/>` on the same member. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Commit `800ec0d fix: suppress S6966 on intentional sync SaveChanges call` — the suppression was applied as a separate post-merge commit because the developer agent's worktree build did not surface S6966, and the main-repo post-merge build did. The rule for running SonarQube before committing did not call out S6966 as a known hazard for sync EF test code. | Added rule: when test code must call the synchronous EF save path intentionally (e.g. `context.SaveChanges()` to exercise a `SavingChanges` override), add `#pragma warning disable S6966` / `#pragma warning restore S6966` around the call before staging — not after merge. Added explicit S6966 callout to the SonarQube self-review checklist item. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Reviewer flagged `SaveCitiesBatchService.PersistAsync` manual stamping as a High finding — this is correctly deferred to task 5.2 but the reviewer had no context to know that. The reviewer prompt contained no list of deferred tasks. | Added orchestrator guidance: when submitting a section for review, always include the list of tasks deferred to later sections (by task ID) and a note that those items are out of scope for the current review. | `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- The first developer agent was killed before its first tool call because `.claude/settings.local.json` in the fresh worktree lacked Read, Glob, Grep, Write, Edit permissions — these are not inherited from the project default. The orchestrator had to patch the file and relaunch the agent. No rule change is possible until the worktree-creation tooling supports seeding permissions from the project default; tracked as a recurring manual step.
