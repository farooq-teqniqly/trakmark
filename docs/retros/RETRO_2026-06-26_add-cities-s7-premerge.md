# Retro — add-cities section 7 (Pre-merge) — 2026-06-26

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Section 7 has no `.T` task and no spec scenarios; the orchestrator had to explicitly tell the developer agent to skip TDD, meaning the agent instructions were silent on this case | Added a leading note to the TDD workflow section: if a section has no `.T` task and cites no spec scenarios (pre-merge or chore), skip the entire TDD workflow and go straight to "Before finishing" | `.claude/agents/developer.md` |
| TDD rule in CLAUDE.md and config.yaml did not mention pre-merge/chore exemption, so the exemption had to be communicated ad-hoc each time | Added sentence to the TDD bullet/paragraph: pre-merge and pure-chore sections with no spec scenarios and no new production behavior are exempt from failing-test-first | `CLAUDE.md`, `openspec/config.yaml` |
| The developer agent "Before finishing" section had no coverage-gate check step; the reviewer correctly applied the exemption but the agent had no self-check prompt | Added step 1 to "Before finishing": run `git diff main --name-only` and confirm whether `Trakmark.Domain` is touched before deciding if the 100% coverage gate applies | `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- Worktree cleanup was clean and fast; no friction observed. Nothing to change.
- tasks.md pre-consolidation was straightforward with sections 1-6 already in sync. No rule gap identified.
