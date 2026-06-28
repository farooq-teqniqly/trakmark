# Retro — identity-user-mapping Section 5 — 2026-06-28

Wire AddCities and remove manual stamping

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| `// (setup in constructor)` annotation on the `// Arrange` line slipped through two review rounds | Extended the AAA marker rule: no explanatory comment on the line immediately following a marker either; if the constructor provides full setup, write `// Arrange` alone | CLAUDE.md, openspec/config.yaml, .claude/agents/developer.md |
| `RendererInfo.IsInteractive` guard was a Medium round-1 finding with no existing guidance | New rule: every `InteractiveServer`/`InteractiveAuto` component with a side-effectful `OnInitializedAsync` must check `RendererInfo.IsInteractive` first | CLAUDE.md, openspec/config.yaml, .claude/agents/developer.md |
| Narrow exception filter (`when (ex is InvalidOperationException or FormatException)`) let unexpected types bypass the error display; round-2 flagged it | New rule: prefer broad `catch (Exception ex)` in `OnInitializedAsync`, emit a `[LoggerMessage]` Warning+, set a user-visible error field | CLAUDE.md, openspec/config.yaml, .claude/agents/developer.md |
| `_isSaving = true` without `StateHasChanged()` meant the button never visually disabled; round-2 finding | New rule: call `StateHasChanged()` immediately after setting an in-flight flag in a Blazor Server component | CLAUDE.md, openspec/config.yaml, .claude/agents/developer.md |

## Carry-forward (not yet actionable)

- Fix agent session-limit failure (round-1 agent produced no output and required an identical retry): this is an infrastructure/platform constraint, not a rule gap — no rule change applied.
