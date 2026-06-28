# Retro — identity-user-mapping section 7 — 2026-06-28

Section: CLAUDE.md rules

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| The section 7 commit (`815d251`) updated only `CLAUDE.md` and `tasks.md`; `openspec/config.yaml` received neither rule 7.1 nor rule 7.2, leaving the two files out of sync | Added both missing rules to `config.yaml` under `rules.tasks` | `openspec/config.yaml` |
| Reviewer (Info) noted that rule 7.2 does not specify `DateTimeOffset` as the required type for `CreatedAt`; a developer adding a new `IAuditableEntity` could use `DateTime` and the rule would not prevent it | Extended rule 7.2 to explicitly state: `CreatedAt` must be typed `DateTimeOffset` (not `DateTime`); the interceptor stamps it with `DateTimeOffset.UtcNow` | `CLAUDE.md`, `openspec/config.yaml` |
| Developer agent self-review checklist in `developer.md` had no items covering dedicated entity configuration classes or audit field ownership/type; violations of rules 7.1 and 7.2 would not be caught before staging | Added two checklist items: "Dedicated entity configuration classes" and "Audit fields — type and ownership" | `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- Rule 7.1 hard-codes `Trakmark/Data/Configurations/` as the configuration class path. If a second bounded context is introduced, this path assumption may need revisiting. No rule change applied — the project currently has one bounded context and the correct path is unambiguous.
