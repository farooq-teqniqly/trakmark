# Retro — identity-user-mapping section 6 — 2026-06-28

Section: AuditInterceptor and ICurrentUserContext tests

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| `Microsoft.EntityFrameworkCore.InMemory` added via `dotnet add package` in a worktree was never committed in the worktree branch; showed as unstaged in the main repo at merge time, blocking the `git merge` and requiring an out-of-band commit | Added self-review step: after `dotnet add package` in a worktree, verify the `.csproj` change is staged and committed in the worktree branch before finishing | `.claude/agents/developer.md` |
| EF InMemory package requirement was undocumented, so agents were unaware they needed to explicitly add it before writing interceptor unit tests | Added rule: interceptor unit tests require `Microsoft.EntityFrameworkCore.InMemory` in the test project `.csproj`; add the package before writing tests | `CLAUDE.md`, `openspec/config.yaml` |
| All 3 async AuditInterceptor scenarios had tests but only the happy-path sync scenario did; null-UserId sync path was missing, caught as a Medium finding requiring one fix round | Added rule: when an interceptor overrides both `SavingChangesAsync` and `SavingChanges`, every test scenario must be mirrored on both the async and sync paths | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Trailing inline comment on `#pragma warning disable S6966` restated what the suppression does, violating the no-restatement convention (Low finding) | Extended the existing S6966 pragma rule to explicitly forbid trailing inline comments on `#pragma warning disable` lines | `CLAUDE.md`, `.claude/agents/developer.md` |

## Carry-forward (not yet actionable)

- High unit/integration test overlap (all 4 unit scenarios mirrored 1:1 in integration tests) was noted as a maintenance risk. No rule change applied — the overlap was not a blocker and the correct boundary between unit and integration coverage needs a concrete failure pattern before a rule is warranted.
