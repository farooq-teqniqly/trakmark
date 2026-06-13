# Retro — test-improvements (sections 11–13) — 2026-06-10

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Section 13 tasks.md said "or a new `Ids/DomainIdTests.cs`" for a task targeting `DomainId.IsValid`, which is `internal`. The cleaner approach — testing through `TryParse` in the existing file — had to be recognized manually. No rule existed to prevent the misleading task wording or the impulse to create a direct test. | Added rule: do not test `internal` helpers directly; cover them through their public API callers. Added matching tasks rule: when generating a task for an internal member, instruct the agent to test via the public caller rather than suggesting a new file. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- Sections 11–13 were all small (1–2 test additions each) with no production code changes; no reviewer was spawned and all 289 tests passed on first run. No review-caught issues to carry forward.
