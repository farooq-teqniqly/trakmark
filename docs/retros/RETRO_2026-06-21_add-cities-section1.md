# Retro — add-cities (section 1: Domain: State) — 2026-06-21

## Context

Implementation of section 1 (`State` value object) of the `add-cities` OpenSpec
change, run via the `trakmark-engineering-team` orchestrator with worktree
isolation (`C:\src\my\trakmark-section1`). Code review was clean (Low/Info
findings only) — no fix round needed. All observed friction was
tooling/permissions related across the developer agent (x2 launches) and the
pr-reviewer subagent, not a code-quality or convention gap.

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Developer agent's first invocation in a fresh worktree had **all** tool calls denied (Bash, Read, Glob) despite `.claude/settings.local.json` being patched with `Write, Edit, Bash(git *), Bash(dotnet *)`. `Read`/`Glob`/`Grep` were never included in the patch template, so they fell back to deny in the worktree's isolated settings context instead of the expected default-allow. | Updated the worktree settings patch template to explicitly include `Read`, `Glob`, `Grep` alongside the existing entries. Added a note explaining that default-allowed tools are *not* inherited automatically by a fresh worktree's isolated settings and must be explicit. | `.claude/agents/trakmark-engineering-team.md` |
| The patch step ran *after* launching agents ("After launching all agents, immediately patch...") — a race window where the agent's first tool call could fire before the settings file was written/read. | Reordered the instruction to patch settings **before** spawning the worktree's subagent. | `.claude/agents/trakmark-engineering-team.md` |
| Developer agent's second run still had Edit/Read/Glob denied despite Edit being explicitly allow-listed; worked around it via `Write` + `git apply` patch file. Re-launching with the corrected, fuller allow-list (including Read/Glob/Grep) addresses the most likely cause — a stale/incomplete settings snapshot from the first denied launch. | Added explicit guidance: if a subagent's first tool call is denied despite a correctly patched settings file, re-verify the file and relaunch once in the existing worktree (no new isolation) before treating it as a deeper failure. | `.claude/agents/trakmark-engineering-team.md` |
| Orchestrator (running in main repo) could not `Read`/`Bash` into the worktree path at all (only `.claude` subdirectory was reachable), forcing it to trust developer self-reports and delegate verification entirely to the pr-reviewer subagent. This was undocumented behavior, not a bug. | Added a guardrail explicitly stating the orchestrator does not have direct filesystem/Bash access into spawned worktrees beyond `.claude`, and should rely on the pr-reviewer subagent for independent verification rather than attempting direct access. | `.claude/agents/trakmark-engineering-team.md` |
| pr-reviewer subagent hit the same Read/Glob denial pattern in its worktree session, working around it with `git show`/`git ls-files` + `dotnet test`. Same root cause as the developer agent's denial — fixed by the same settings template change (the orchestrator patches worktrees once; both developer and pr-reviewer subagents operate in the same patched worktree). | Consolidated into the single settings-template fix above — no separate change needed since both subagent types read the same worktree-local `.claude/settings.local.json`. | `.claude/agents/trakmark-engineering-team.md` |

No changes were made to `CLAUDE.md`, `openspec/config.yaml`, or
`.claude/agents/developer.md` — this cycle's friction was entirely
tooling/permissions related (worktree settings propagation), not a coding
convention or workflow-step gap in the developer agent itself. The review was
clean with no fix-round findings, so there were no code-quality patterns to
encode as rules.

## Carry-forward (not yet actionable)

- Root cause of the first-launch *total* denial (point (a) vs (b) in the task
  description — race condition vs. missing explicit grants) was not
  conclusively isolated; the explicit `Read`/`Glob`/`Grep` grant plus
  patch-before-launch ordering address both plausible causes, but if the
  issue recurs after this fix, it would indicate the race condition (a) is
  the dominant cause and may need a verification/retry loop added to the
  orchestrator's Step 1 (e.g. a no-op `Read` call immediately after spawn to
  confirm settings are live before issuing real work).
- Consider whether the underlying tool-permission system should treat
  Read/Glob/Grep as truly default-allow even in isolated worktree contexts,
  removing the need for explicit allow-listing entirely. This is an
  environment-level concern outside the scope of rule-file edits.
