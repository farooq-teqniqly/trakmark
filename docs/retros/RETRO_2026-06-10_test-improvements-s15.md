# Retro — test-improvements (section 15) — 2026-06-10

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Task 15.1 said "Affected types: all types in `Catalog/`, `ValueObjects/`, `Aggregates/`, and `Ids/`" for null-left `==` assertions. ID types are `readonly record struct` — the compiler rejects a typed null as a value-type operand, making the null-guard branch structurally unreachable. The agent had to recognize this and skip `Ids/` without any rule to guide it. A future agent generating or executing such a task would blindly include value types and produce a compile error. | Added explicit rule: when writing `Assert.False(null == x)` assertions, skip `readonly record struct` types — only reference-type (`sealed class`) value objects need null-left coverage. | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- No reviewer or worktree was needed for this purely mechanical, assertion-append change (no new test methods, no production code). The `trakmark-engineering-team` agent mandates reviewer + retro regardless of section count, which is intentional — this cycle's smoothness validates that rule is not a burden for small changes.
- Task 16.1 (branch coverage verification) is not yet complete; no retro finding applies until that result is known.
