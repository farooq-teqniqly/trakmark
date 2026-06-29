# Retro — identity-user-mapping section 3 (Registration hook) — 2026-06-27

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| 6 violations of AAA-marker inline notes in a single test file (Medium, round 1). Rule existed in CLAUDE.md and config.yaml but was absent from the developer self-review checklist. | Added **AAA marker cleanliness** checklist item: scan every test method for trailing text on AAA markers before staging. | `.claude/agents/developer.md` |
| Test method named `CreateAndSignInUserAsync_...` instead of the observable entry point `ExternalLoginCallback_...` (Medium, round 1). Roy Osherove naming rule did not say Subject must be a public entry point. | Sharpened Roy Osherove naming rule: the Subject segment must name the observable public entry point, not a private/internal helper. Added matching checklist item **Test method Subject is the public entry point**. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| `catch (DbUpdateException)` idempotent-success path had no test coverage (High, round 1). Checklist had discriminated-union completeness but no prompt for catch branches. | Added rule: every `catch` block that suppresses or transforms an exception must have at least one dedicated test case. Added matching checklist item **Catch branch test coverage**. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| `Assert.Equal(identityUserId, entity.AccountId)` after `SingleAsync(r => r.AccountId == identityUserId)` flagged as redundant (Low, round 1). No rule existed. | Added rule: do not assert a property value that the query predicate already guarantees. | `CLAUDE.md`, `openspec/config.yaml` |
| `Assembly.GetType(string)!` used instead of `?? throw` (Low, round 1). No rule existed for null-forgiving on reflection results. | Added rule: never use `!` on reflection results; use `?? throw new InvalidOperationException(...)` instead. | `CLAUDE.md`, `openspec/config.yaml` |
| Silent catch block — idempotent-success catch emitted no log (Low, round 1). Source-generated logging rule covered production calls but not catch blocks. | Added rule: every catch block that suppresses or re-routes an exception must emit at least one `[LoggerMessage]`-generated log at Warning or above. | `CLAUDE.md`, `openspec/config.yaml` |

## Carry-forward (not yet actionable)

- None. All six findings have been codified.
