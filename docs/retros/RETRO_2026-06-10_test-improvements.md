# Retro — test-improvements — 2026-06-10

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| TDD "write failing test first" rule gave no guidance for test-only changes where production code already exists — the failing-first step was unobservable and therefore skipped | Added explicit guidance: for test-only changes, write each test body with `Assert.Fail("not implemented")`, confirm it fails, then replace with real assertions | `CLAUDE.md`, `openspec/config.yaml` |
| `openspec status --json` output key is `changeRoot` (a fully resolved absolute path) but developer agent Step 0 did not document this, leaving runners to guess the path construction | Added a clarifying note to Step 0 documenting that `changeRoot` is the fully resolved absolute path and should be used directly to construct artifact paths | `.claude/agents/developer.md` |
| Developer agent had worktree cleanup instructions but no guidance on when worktrees are *not* needed, creating ambiguity for single-section and test-only runs | Added an "Orchestrator: when to use worktrees" section stating worktrees are optional for single-section and test-only changes | `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- No reviewer findings were surfaced for this change (all 255 tests passed, no post-review fix commits). Nothing to carry forward from review.
