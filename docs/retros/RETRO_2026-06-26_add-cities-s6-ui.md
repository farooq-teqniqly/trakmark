# Retro — add-cities §6 UI: Add Cities page — 2026-06-26

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Fixing spurious `StateHasChanged()` calls exposed S2325 warnings that had been masked, causing an avoidable round 3 | Added "SonarQube clean before committing" item to self-review checklist: run the build filter before staging, not only at merge time | `.claude/agents/developer.md` |
| Reviewer wrote `docs/PR_REVIEW_section6-ui.md` despite explicit "reply in chat only" instruction in the prompt | Added an **Override** notice at the top of the Output file section: if the calling prompt says "reply in chat only" or "do not write a file", skip the file write entirely | `.claude/agents/pr-reviewer.md` |
| Developer used `maxlength="101"` instead of 100; correct value was in the domain type's constructor guard | Added "UI form constraints vs domain constraints" item to self-review checklist, and mirrored as a rule in CLAUDE.md (UI testing) and openspec/config.yaml (ui_testing) | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Developer covered 3 of 5 `SaveCitiesBatchResult` subtypes in tests | Added "Discriminated union test completeness" item to self-review checklist, and mirrored as a rule in CLAUDE.md (testing) and openspec/config.yaml (tasks) | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Developer wrote 3 separate `[Fact]`s for nav-visibility tests despite the existing `[Theory]` rule | Strengthened the rule from backward-looking ("delete redundant facts after a Theory exists") to forward-looking ("the moment you need a second `[Fact]` with different inputs, stop and write a `[Theory]`") | `CLAUDE.md`, `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- The SonarQube self-check step in developer.md uses a PowerShell pipeline; on non-Windows environments the command would differ. If the project ever runs agents on Linux, the command needs a cross-platform variant.
- The pr-reviewer override relies on the calling prompt using specific phrasing. A more robust solution would be a formal `--output chat` flag on the agent invocation, which the SDK does not currently support.
