# Retro — add-cities, Section 3: Domain: City — 2026-06-21

## Context

Implementation of section 3 (`City` value object: `sealed class City :
IEquatable<City>` with `CityId`, `Name`, `State`; `Create` factory) of the
`add-cities` OpenSpec change, depending on the already-merged `State` and
`CityId` types from sections 1–2. Round-1 review found one Medium finding
(missing trim-and-accept test for `City.Create`); a fix agent added the
test and round-2 incremental re-review confirmed the fix with no new
issues. tasks.md consolidated and merged cleanly (`--no-ff`); full suite
green post-merge (359/359). Worktree/branch cleanup succeeded (one
transient "Permission denied" on `git worktree remove`, resolved on retry
— consistent with the same transient noted in the section 2 retro, still
not systemic enough to need a rule).

`City.cs` itself has 100% line/branch coverage. The reviewer separately
flagged a **pre-existing** `Trakmark.Domain` coverage gap in `Meet.cs`
(98.79%) that predates this branch (confirmed via empty `git diff` of
`Meet.cs` between `add-cities` and the worktree) — out of scope for
section 3, carried forward.

## Key friction: worktree/branch mismatch (this cycle's primary finding)

The orchestrator pre-created a worktree at `C:\src\my\trakmark-section3` on
branch `add-cities-section3` and patched its `.claude/settings.local.json`
*before* spawning the developer agent, per the orchestrator skill's
documented flow at the time. The developer agent's tools (Bash, Read,
Glob, Grep) could not access that path at all — every call against it was
denied. Instead, the Agent tool's `isolation: "worktree"` parameter had
already auto-created its own worktree and branch
(`C:\src\my\trakmark\.claude\worktrees\agent-a4905682645909aa6` on
`worktree-agent-a4905682645909aa6`, based on the same `add-cities` commit),
and the agent did all its work there: `City.cs`, `CityTests.cs`, and
`tasks.md` updates. The pre-created `trakmark-section3` worktree was never
used and had to be discarded; the orchestrator had to merge from the
agent's actual auto-assigned branch instead of the one named in the
prompt, discovered only after the fact.

This is **not a one-off**. Confirmed recurring across two prior cycles:
- `trakmark-domain-model-pilot` (2026-06-05): commit `1c820bf` —
  `On domain-modelling: Checking out worktree-agent-a4417bb67ba549d93`.
- `add-cities` §3 (this cycle): `worktree-agent-a4905682645909aa6`.

Sections 1 and 2 of `add-cities` did not surface this as a *named* finding
in their retros — section 1's retro focused on settings-propagation
denials inside whatever worktree the agent ended up in, and section 2's
retro focused on `cd`-vs-allow-list Bash denials — but neither diagnosed
*why* a pre-created worktree was needed in the first place. The root cause
was the same underlying mismatch in all three sections; it only became an
explicit, isolated finding in §3 because the orchestrator happened to name
and patch a worktree path that turned out to be completely unused.

The orchestrator skill (`trakmark-engineering-team.md`) Step 1 told the
orchestrator to patch "every newly created worktree's settings" before
spawning developer agents, implying a pre-creation step, but never actually
documented running `git worktree add` — meaning every orchestration run
had to improvise a pre-creation step that conflicts with how
`isolation: "worktree"` actually behaves (it always creates and owns its
own worktree/branch, ignoring any path/branch named in the prompt text).

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Orchestrator pre-created `C:\src\my\trakmark-section3` / branch `add-cities-section3` and patched its settings, but the `isolation: "worktree"` Agent tool spawn ignored it entirely and auto-created its own worktree/branch (`.claude/worktrees/agent-a4905682645909aa6` / `worktree-agent-a4905682645909aa6`); confirmed as a recurring pattern dating back to the `trakmark-domain-model-pilot` cycle (`worktree-agent-a4417bb67ba549d93`). | Removed the implied pre-creation step from Step 1. Added explicit guidance: never run `git worktree add` or invent a worktree path/branch name before spawning an `isolation: "worktree"` subagent; instead spawn directly, then discover the Agent tool's actual auto-assigned path/branch from the spawn result and use that everywhere downstream (reviewer prompts, fix agents, coverage checks, merge, cleanup). | `.claude/agents/trakmark-engineering-team.md` |
| The settings-patch instructions said to patch the worktree "before launching any agents" / "before spawning" — but a pre-created worktree is now explicitly disallowed, and the auto-created worktree does not exist until the agent spawns, so pre-spawn patching is impossible. The first-call-denied-then-relaunch workaround already existed but was framed as a fallback for a missed grant rather than the expected first step. | Rewrote the settings-patch section as a numbered sequence: (1) once the auto-created worktree path is known, patch its settings; (2) relaunch the same prompt with no new `isolation` (reusing the existing worktree) to pick up the patched settings; (3) only treat continued denial as a deeper failure. Reframed the first-call denial as expected behavior, not a fallback case. | `.claude/agents/trakmark-engineering-team.md` |
| The Guardrails section had no rule capturing the path/branch-discovery requirement, so a future orchestrator run could repeat the same pre-creation mistake even after Step 1 was corrected. | Added a guardrail bullet: never plan or reference a worktree path/branch before spawning; always discover and use the actual auto-assigned `.claude/worktrees/agent-<id>` path and `worktree-agent-<id>` branch from the spawn result for every downstream step. | `.claude/agents/trakmark-engineering-team.md` |

No changes to `CLAUDE.md` or `openspec/config.yaml` — this cycle's
findings were entirely an orchestration/workflow gap in how the
orchestrator coordinates with the Agent tool's isolation mechanism, not a
coding convention. The `City.Create` trim-test gap (round-1 review finding)
was a one-off test-coverage miss already correctly caught by the existing
review cycle and fixed within process — it does not indicate a missing
rule (the existing "test behavior, not implementation" / scenario-coverage
conventions already cover this case; the gap was an oversight, not absence
of a rule).

## Carry-forward (not yet actionable)

- `Trakmark.Domain` coverage gap in `Meet.cs` (98.79%, pre-existing,
  unrelated to City) still blocks the eventual 100%-domain-coverage gate
  for the `add-cities` → `domain-modelling`/`main` merge. Needs a dedicated
  fix (likely a small developer task to cover the missing `Meet.cs`
  branch) before that merge, independent of any `add-cities` section work.
- Transient `git worktree remove --force` "Permission denied" on first
  attempt, succeeding on retry — observed again this cycle (also noted in
  the section 2 retro). Still treating as an OS-level file-lock timing
  issue rather than a systemic gap; would need a rule (e.g. automatic
  retry-with-backoff in Step 6) only if it starts blocking cleanup
  outright instead of self-resolving on retry.
