# Retro — trakmark-domain-model-pilot — 2026-06-05

Sections 3–6 implemented in parallel worktrees, reviewed per-section,
merged into `domain-modelling`. 126 tests green post-merge.

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| §3, §4, §5 all shipped `sealed class` without `==`/`!=` operators; caught in review each time | Added `==`/`!=` operator overloads to the sealed-class value-object rule | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| `ArgumentNullException.ThrowIfNull` missing on ctor/method params in §3, §4, §5; caught by reviewer in all three | Added null-guard rule: `ThrowIfNull` on all public ctor/method ref-type params | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| §3 `GetHashCode` used default string hash while `Equals` used `StringComparison.Ordinal`; silently breaks dict lookups | Added comparer-parity rule: `GetHashCode` must use same `StringComparison` as `Equals` | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| §5 referenced §4-owned types (`Career`, `Student`, `Enrollment`) using invented APIs (`.Grade`, `.AccountLink`, wrong ctor signature); required post-merge fixup commit `71bdefb` | Added TDD workflow step: when referencing a prior-section type, use only its merged public API; never invent a stub API | `.claude/agents/developer.md` |
| Fix agents spawned as `cavecrew-builder` had no Bash; blocked on `dotnet test` and `git commit`; main thread had to commit manually for §4, §5 | Added guidance: fix agents must use `subagent_type: developer` | `.claude/agents/retrospective.md` |
| Worktree `.claude/settings.local.json` files lacked `Write`, `Edit`, `Bash(git *)`, `Bash(dotnet *)`; all developer agents blocked on file creation and commits | Added those entries to project-level `.claude/settings.local.json` permanently | `.claude/settings.local.json` |
| Stale filename reference `openspec-developer.md` in retrospective.md (renamed to `developer.md` in commit `7ad662a`) | Corrected filename reference | `.claude/agents/retrospective.md` |
| Post-merge: stale worktree directories and branches left on disk/branch list | Added post-merge cleanup rule: `git worktree remove --force` + `git branch -d` | `.claude/agents/developer.md` |

| `tasks.md` conflicted on every merge because all worktrees checked off their own tasks independently | Pre-consolidate tasks.md on target branch before first merge; resolve subsequent conflicts with `--ours` | `.claude/agents/trakmark-engineering-team.md` |

## Carry-forward (not yet actionable)

- ~~`StringComparison.Ordinal` for user-typed name values~~ — resolved: switched to `OrdinalIgnoreCase` (decision confirmed by owner).
- `Team` has no `SchoolId` back-reference — noted by reviewer; relevant when EF Core persistence is added.
- `Student.AccountLink` private set has no internal mutator — unreachable until a `LinkAccount` operation is designed.
- ~~`StringComparison.Ordinal` for user-typed name values~~ — resolved: switched to `OrdinalIgnoreCase` (decision confirmed by owner).
- `Team` has no `SchoolId` back-reference — noted by reviewer; relevant when EF Core persistence is added.
- `Student.AccountLink` private set has no internal mutator — unreachable until a `LinkAccount` operation is designed.

## Process additions

- Created `.claude/agents/trakmark-engineering-team.md` — orchestrator skill; invoke as `/trakmark-engineering-team <change> <sections>` to run the full parallel workflow.
- Retrospective output now written to `docs/retros/` and committed alongside rule edits.
