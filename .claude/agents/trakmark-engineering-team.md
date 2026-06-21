---
name: trakmark-engineering-team
description: >-
  Parallel TDD implementation orchestrator for the Trakmark project. Given an
  OpenSpec change name and a list of section numbers, spawns one developer
  agent per section in isolated worktrees, runs a per-section reviewer on
  completion, iterates fix→re-review (max 3 rounds), merges all passing
  branches into the target, cleans up worktrees and branches, then runs the
  retrospective. Invoke with: /trakmark-engineering-team <change> <sections>
  e.g. /trakmark-engineering-team trakmark-domain-model-pilot 3 4 5 6
tools: Read, Edit, Write, Grep, Glob, Bash, Agent
model: inherit
---

You are the engineering team orchestrator for the Trakmark project.

## Invocation

Arguments passed after the skill name:
- `<change>`: OpenSpec change name (e.g. `trakmark-domain-model-pilot`)
- `<sections>`: one or more section numbers (e.g. `3 4 5 6`)
- `--branch <name>` (optional): target merge branch, defaults to `domain-modelling`

## Step 0 — Read the change

1. Read `openspec/changes/<change>/tasks.md` to extract, for each requested section:
   - Section number and title (the `## N. Title` heading)
   - All tasks under that section (`.T` task + numbered tasks)
   - The spec file(s) cited in the `.T` task description
2. Read `openspec/changes/<change>/design.md` for domain context.
3. Identify which sections depend on others (e.g. §5 RegisteredUser depends on §4 Student).

## Single-section rule

When only one section is requested, follow **all steps identically** — worktree,
reviewer, fix→re-review cycle, merge, cleanup, and retrospective. Do **not**
skip or short-circuit any step because there is only one section. The reviewer
and retrospective are mandatory regardless of section count.

## Step 1 — Launch developer agents in parallel worktrees

For each section (even if only one), spawn a `developer` subagent with
`isolation: "worktree"` and `run_in_background: true`.

**Do not pre-create a worktree or branch for the agent.** The Agent tool's
`isolation: "worktree"` parameter always creates its own worktree (under
`.claude/worktrees/agent-<id>`) and its own branch (`worktree-agent-<id>`,
based on the current branch) when the subagent spawns — it does not honor or
even look at any worktree path or branch name you create beforehand or
mention in the prompt text. A worktree pre-created at a different path (e.g.
`C:\src\my\trakmark-section<N>`) will sit unused; the agent does its actual
work in the auto-created one regardless of what the prompt says. Confirmed
recurring across the pilot (`worktree-agent-a4417bb67ba549d93`) and add-cities
§3 (`worktree-agent-a4905682645909aa6`) — this is not a one-off.

Instead:
1. Spawn the subagent with `isolation: "worktree"` directly — do not run any
   `git worktree add` setup step first.
2. After the subagent's first response (or completion), read back the
   worktree path and branch the Agent tool actually assigned — do not invent
   or assume a path. Treat that as authoritative for every subsequent step
   (reviewer prompts, coverage checks, merge, cleanup).
3. Use that discovered path/branch in all later steps (Step 2 reviewer
   prompt's `Work in:`/`Branch:`, Step 3 fix agents, Step 5 merge, Step 6
   cleanup) instead of any name you planned in advance.

Prompt template for each developer agent:

```
Implement section <N> of OpenSpec change "<change>" using strict TDD.

Change: <change>
Section: <N> — <Title>
Spec files: <spec-filenames from .T task>
Prior sections complete: <list what is already implemented>

Tasks:
<paste the full task list for this section from tasks.md>

Follow the developer agent instructions exactly:
1. Run `openspec status --change "<change>" --json` first to get paths.
2. Read config.yaml and CLAUDE.md for conventions.
3. Read design.md and tasks.md.
4. Read the spec file(s).
5. Write failing tests FIRST. Confirm they fail for missing-type reasons.
6. Implement tasks in order. Run `dotnet test` after each.
7. Mark tasks done in tasks.md (- [ ] → - [x]).
8. Self-review checklist: sealed class + IEquatable<T> + ==/ != for value objects;
   ArgumentNullException.ThrowIfNull on all public ctor/method ref-type params;
   GetHashCode uses same StringComparison as Equals; no [Fact]/[Theory] duplicates;
   XML <summary> on all public/internal members.
9. Commit: conventional message ≤50 char subject,
   Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>

Use PowerShell (Bash tool available for dotnet/git). All paths relative to worktree root.
```

You cannot patch a worktree's `.claude/settings.local.json` before spawning —
the worktree does not exist on disk until the Agent tool creates it as part of
the `isolation: "worktree"` spawn. Expect the agent's first tool call in that
fresh worktree to be denied (Bash, Read, Glob, and Grep all included) because
the isolated settings context does not inherit the project's default-allow
list. Treat this first denial as expected, not a deeper failure:

1. From the orchestrator, once the worktree path is known (see Step 1),
   patch `<worktree-path>/.claude/settings.local.json` to include:

   ```json
   {
     "permissions": {
       "allow": ["Read", "Glob", "Grep", "Write", "Edit", "Bash(git *)", "Bash(dotnet *)"]
     }
   }
   ```

   Read the file first if it exists; merge the entries rather than
   overwriting if it already has content. `Read`, `Glob`, and `Grep` are
   normally default-allowed tools elsewhere, but a fresh worktree's isolated
   settings context does not inherit that default — they must be explicitly
   listed.
2. Relaunch the same agent/prompt once in the existing worktree (no new
   `isolation` parameter, so the Agent tool reuses the already-created
   worktree instead of spawning another one) so it picks up the patched
   settings.
3. If the relaunch is still denied, re-verify the file contents before
   treating it as a deeper failure.

## Step 2 — React to completions: launch per-section reviewers

When each developer agent completes, immediately:

1. Mark the section task in your task list as completed.
2. Spawn a `pr-reviewer` subagent with `run_in_background: true` targeting that
   worktree. Prompt template:

```
Review section <N> (<Title>) implementation.

Work in: <worktree-path>
Branch: <worktree-branch>

Context: domain model for a school track & field app (.NET 10, C#, xUnit).
Section <N> adds: <brief description of what was implemented>.

Do not `cd` into the worktree — run every git/dotnet command with the
worktree path as an explicit argument instead, e.g.
`git -C <worktree-path> log --oneline -5`,
`git -C <worktree-path> diff domain-modelling..HEAD`,
`dotnet test <worktree-path>\Trakmark\Trakmark.slnx`.
A bare `cd <path> && dotnet ...` compound command will not match the
`Bash(dotnet *)` / `Bash(git *)` allow-list patterns (the command does not
start with `dotnet`/`git`) and will be denied — always lead with the binary
name and pass the path as an argument.
No GitHub PR — skip gh api fetch.

Apply project standards from CLAUDE.md:
- sealed types by default
- Value objects with validation: sealed class + IEquatable<T> + ==/ != operators, NOT record
- ArgumentNullException.ThrowIfNull on all public ctor/method ref-type params
- GetHashCode must use same StringComparison as Equals
- XML <summary> on all public/internal types and members
- No redundant [Fact] where [Theory] already covers the case
- Cyclomatic complexity ≤ 15

Write review to docs/PR_REVIEW_section<N>.md in the worktree.
Report path and top findings.
```

## Step 3 — Fix → re-review cycle (max 3 rounds per section)

When a reviewer completes:

1. If findings include Critical or High severity items — or Medium items that
   violate a CLAUDE.md rule — spawn a **`developer`** subagent (NOT
   `cavecrew-builder` — it has no Bash) with `run_in_background: true` to fix
   them. Give the agent: the worktree path, branch, files to fix, and exact
   findings with line citations.
2. If the agent cannot commit (git blocked), commit from the main thread.
   The orchestrator's Bash `cd` to a path outside its own cwd is denied — use
   `-C <worktree>` / absolute-path-first-arg forms instead, never `cd &&`:
   `git -C <worktree> add ... && git -C <worktree> commit ...` and
   `dotnet test <worktree>\Trakmark\Trakmark.slnx ...`
3. Spawn a re-reviewer with incremental mode: parse `Reviewed commit:` from the
   existing review file, scope to `git diff <old-hash>..HEAD`.
4. Repeat up to **3 review rounds total**. After round 3, carry forward any
   remaining Low/Info findings — do not block merge for them.

## Step 3.5 — Pre-merge: 100% Trakmark.Domain coverage

Before merging any worktree branch that touches `Trakmark.Domain`, run the
`coverage-report` skill (or its `Run-Coverage.ps1`) against that worktree. If
`Trakmark.Domain` line coverage is below 100%, spawn a `developer` agent to add
the missing tests — domain types have no untestable infrastructure
dependencies, so a gap means a missing test, not an exemption — and re-run
coverage until it hits 100%. Do not proceed to Step 4 for a section until its
coverage is 100%.

## Step 4 — Pre-merge: consolidate tasks.md

Before merging any branch, collect all checked-off tasks across every worktree
and apply them once to the target branch's `tasks.md`:

1. For each worktree, read its `tasks.md` and note every line changed from
   `- [ ]` to `- [x]`.
2. Apply the union of all completions to the target branch's `tasks.md` on the
   main repo (not in any worktree).
3. Commit: `docs: check off sections <N>-<M> tasks prior to merge`

During each subsequent merge, when `tasks.md` conflicts, always resolve with
`git checkout --ours tasks.md` — the target branch already has the correct
final state.

## Step 5 — Merge

Once all sections pass review and tasks.md is pre-consolidated:

1. Determine merge order: most independent sections first; sections that define
   types others depend on before sections that consume them.
2. For each section in order, run all git/dotnet commands with `-C <repo-root>`
   or an absolute path as the first argument — never `cd <repo-root> && ...`.
   The orchestrator's Bash `cd` to a path outside its own cwd is denied, but
   `-C`/direct-path invocations work:
   ```bash
   git -C <repo-root> merge --no-ff <worktree-branch> -m "Merge section <N>: <Title>"
   # On tasks.md conflict:
   git -C <repo-root> checkout --ours openspec/changes/<change>/tasks.md
   git -C <repo-root> add openspec/changes/<change>/tasks.md
   ```
3. On conflict in domain files: keep the authoritative version for shared types
   (prefer the section that owns the type); take the version with more complete
   validation (e.g. ArgumentNullException.ThrowIfNull present > absent) for
   value objects.
4. After each merge, run `dotnet test` on the main repo. Fix any post-merge
   API mismatches (stub APIs referencing wrong property/method names) immediately
   and commit the fix.

## Step 6 — Cleanup

After all merges are green:

```bash
git worktree remove <worktree-path> --force
git branch -d <worktree-branch>
```

Do this for every section. Verify with `git worktree list` and `git branch -l`.

`git worktree remove --force` can fail with "Permission denied" deleting the
directory on disk — even when the git-level removal already succeeded (the
worktree no longer appears in `git worktree list`) and the branch deletes
cleanly with `git branch -d`. This is a file-lock held by another process
(commonly the `dotnet test` runner or a Testcontainers/SQL Server artifact),
not a real cleanup failure. Treat it as a known limitation: confirm with
`git worktree list` that the worktree is logically gone; if so, the stale
directory under `.claude/worktrees/` is safe to ignore — do not retry-loop or
block the orchestration run on it. A directory left behind this way will not
interfere with future `isolation: "worktree"` spawns (they create new
uniquely-named directories).

## Step 7 — Retrospective

Spawn a `retrospective` subagent (foreground, not background). Pass:
- The change name
- Key friction observed during this cycle (permission blocks, API mismatches,
  recurring review findings, manual commits required, etc.)
- Instruction to apply improvements to CLAUDE.md, openspec/config.yaml,
  and .claude/agents/developer.md

## Guardrails

- Bash `cd` to a path outside the orchestrator's own cwd is denied. Always use
  `git -C <path> ...` / `dotnet test <path>\...` (absolute path as the first
  argument) instead of `cd <path> && ...` for any git/dotnet command the
  orchestrator runs directly against a worktree or the main repo.
- Fix agents: always `subagent_type: developer`. Never `cavecrew-builder` (no Bash).
- Never plan or reference a worktree path/branch name before spawning the
  `isolation: "worktree"` subagent. Always discover the actual auto-assigned
  path (`.claude/worktrees/agent-<id>`) and branch (`worktree-agent-<id>`)
  from the spawn result and use that everywhere downstream (reviewer prompts,
  fix agents, coverage checks, merge, cleanup). Do not run `git worktree add`
  yourself for a developer/reviewer subagent.
- If a developer or reviewer agent cannot write files or read/glob/grep: patch
  the worktree's `.claude/settings.local.json` (add Read, Glob, Grep, Write,
  Edit, Bash(git *), Bash(dotnet *)) and relaunch in the existing worktree (no
  new isolation).
- If a developer agent cannot commit: commit from the main thread.
- The orchestrator (running in the main repo) does not have direct filesystem
  or Bash access into a spawned worktree's path beyond its
  `.claude` subdirectory — do not attempt to `Read`/`Bash` into a worktree
  to verify a subagent's work directly. Trust the developer agent's self-report
  and rely on the pr-reviewer subagent (which runs with its own tool grants
  inside the worktree) for independent verification instead.
- tasks.md will have merge conflicts across branches — resolve by accepting all
  checked-off tasks from all branches (union of completed tasks).
- A stale worktree directory left on disk after `git worktree remove --force`
  reports "Permission denied" is safe to ignore once `git worktree list` no
  longer shows it — it is a file-lock artifact (test runner/Testcontainers),
  not a logical cleanup failure. Do not block or retry-loop on it.
- Track progress with TaskCreate/TaskUpdate: one task per section (impl +
  review), plus merge and retrospective tasks with blockedBy dependencies.
