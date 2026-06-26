# Retro — add-cities, Section 4: Persistence — 2026-06-21

## Context

Implementation of section 4 (EF Core persistence for `City`: `CityEntity`
DTO, `CityConfiguration` with an `int IDENTITY` clustered surrogate key and
`CityId` as a unique non-clustered alternate key, an EF Core migration, and a
new `Trakmark.IntegrationTests` project with a Testcontainers SQL Server
round-trip test) of the `add-cities` OpenSpec change. Single developer agent,
worktree isolation. Round-1 review found zero Critical/High/Medium findings —
the first single-round-clean review observed across this project's cycles so
far — with one Low-severity finding (undocumented rationale for a
`PendingModelChangesWarning` suppression in `ApplicationDbContext`), fixed
directly by the orchestrator with a one-line comment rather than spawning a
full fix-agent round. Merge into `add-cities` was clean, no conflicts; full
suite green post-merge (359 domain + 1 integration test, real Testcontainers
SQL Server run, not skipped).

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Developer needed a temporary global `dotnet-ef` tool install (v10.0.8) to scaffold the EF Core migration; this prerequisite was undocumented anywhere, so the agent had to discover it mid-task. | Added a `dotnet-ef` prerequisite note to CLAUDE.md's Configuration section (check `dotnet ef --version`, install via `dotnet tool install --global dotnet-ef` if missing, call out it's a dev-machine tool not a project dependency) and a matching "Persistence/migration prerequisite" step in the developer agent instructions, placed before the TDD workflow so it's checked at the start of any persistence-touching section. | `CLAUDE.md`, `.claude/agents/developer.md` |
| `git worktree remove --force` failed with "Permission denied" deleting the on-disk directory (likely a file lock from the `dotnet test` runner or a Testcontainers SQL Server artifact), even though `git worktree list` showed the worktree already logically removed and the branch deleted cleanly with `git branch -d`. Three stale worktree directories from prior sections were found still present on disk under `.claude/worktrees/`, confirming this is a recurring pattern, not one-off. | Documented the failure pattern in the orchestrator's Step 6 (Cleanup) and added a Guardrails bullet: treat git-level removal (confirmed via `git worktree list`) as the source of truth; a stale on-disk directory after a "Permission denied" is a known file-lock artifact, safe to ignore, and must not block or retry-loop the orchestration run. | `.claude/agents/trakmark-engineering-team.md` |
| Section 4 did not touch `Trakmark.Domain`, so the 100% Domain coverage gate was judged not applicable and skipped. The gate's wording in CLAUDE.md and config.yaml ("Trakmark.Domain line coverage must be 100% before merging") had no scoping language, so a future cycle reading only those two files (not the orchestrator's already-correctly-scoped Step 3.5) could misapply the gate to every section regardless of whether it touches Domain. | Added explicit scoping language to both the CLAUDE.md pre-merge checklist and the config.yaml `pre_merge` rule: the gate applies only when a change's diff touches `Trakmark.Domain`; persistence/application-layer/UI-only sections are exempt, verified by checking the diff rather than assuming. The orchestrator's Step 3.5 already had correct scoping ("Before merging any worktree branch that touches `Trakmark.Domain`") and needed no change. | `CLAUDE.md`, `openspec/config.yaml` |

## Carry-forward (not yet actionable)

- Stale worktree directories already on disk from prior sections
  (`agent-a4905682645909aa6`, `agent-a5d0d34d8ee46481a`, `agent-ae094f6119e74a5d7`)
  remain under `.claude/worktrees/` and are safe to ignore per the new
  guardrail — no automated sweep exists yet. If disk usage becomes a problem,
  consider a periodic manual or scripted sweep of directories not listed in
  `git worktree list`.
- The `PendingModelChangesWarning` suppression rationale was added reactively
  after a Low-severity review finding. No rule changed for this — it was a
  one-off documentation gap (the underlying coding convention, "no comments
  that restate what the code already says," does not require pre-emptive
  investigation write-ups for every analyzer suppression), so it is left as
  an isolated finding rather than a new rule.
